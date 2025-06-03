using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public sealed class CitiesModel(FiftyBestData data) : PageModel
{
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public IReadOnlyCollection<Restaurant> Restaurants { get; private set; } =
        [];
    public IReadOnlyCollection<CityId> Cities => data.Cities.Keys.ReadOnly();

    public void OnGet(string? country, string? city)
    {
        if (country is null || city is null)
            return;

        // Look up the city based on the parameters, so as to avoid cross-site
        // scripting attacks. DON'T TRUST INPUT.
        var id = new CityId(country, city);
        if (city is { } && data.Cities.TryGetValue(id, out var restaurants))
        {
            Country = id.Country;
            City = id.Name;
            Restaurants = restaurants.DistinctBy(r => r.Name).ReadOnly();
        }
    }
}
