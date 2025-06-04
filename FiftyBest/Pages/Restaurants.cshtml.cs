using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace Ploeh.FiftyBest.Pages;

public class RestaurantsModel(FiftyBestData data) : PageModel
{
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public RestaurantName? Name { get; private set; }
    public IReadOnlyCollection<Ranking> Rankings { get; private set; } = [];
    public IReadOnlyCollection<RestaurantId> Restaurants { get; private set; } =
        [];

    public void OnGet(string? country, string? city, string? name)
    {
        if (country is null || city is null || name is null)
        {
            Restaurants = data.Restaurants.Keys
                .DistinctBy(r => r.Name)
                .OrderBy(r => r.Name.Name)
                .ReadOnly();
            return;
        }

        var rName = RestaurantName.FromUrlSegment(name);
        // Look up the restaurant based on the parameters, so as to avoid
        // cross-site scripting attacks. DON'T TRUST INPUT!
        var id = new RestaurantId(new CityId(country, city), rName);
        if (data.Restaurants.TryGetValue(id, out var rankings))
        {
            Country = country;
            City = city;
            Name = rName;
            Rankings = rankings;
        }
    }
}
