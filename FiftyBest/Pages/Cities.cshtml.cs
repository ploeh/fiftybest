using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public sealed class CitiesModel(FiftyBestData data) : PageModel
{
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public IReadOnlyCollection<Restaurant> Restaurants { get; private set; } =
        [];
    public IReadOnlyCollection<(string Country, string City)> Cities =>
        data.Cities
            .Select(kvp => (kvp.Value.Country, City : kvp.Key))
            .OrderBy(t => t.City)
            .ReadOnly();

    public void OnGet(string? country, string? city)
    {
        // Look up the city based on the parameters, so as to avoid cross-site
        // scripting attacks. DON'T TRUST INPUT.
        if (city is { } && data.Cities.TryGetValue(city, out var restaurants))
        {
            Country = restaurants.Country;
            City = city;
            Restaurants = restaurants.Item2.DistinctBy(r => r.Name).ReadOnly();
        }
    }
}
