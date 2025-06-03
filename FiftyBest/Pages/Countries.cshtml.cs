using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public sealed class CountriesModel(FiftyBestData data) : PageModel
{
    public string? Country { get; private set; }
    public IReadOnlyCollection<string> Countries =>
        data.Countries.Keys.ReadOnly();
    public IReadOnlyCollection<string> Cities { get; private set; } = [];

    public void OnGet(string? country)
    {
        if (country is null)
            return;

        // Look up the country in the data dictionary, so as to avoid cross-
        // site scripting attack. DON'T TRUST INPUT!
        if (data.Countries.TryGetValue(country, out var cities))
        {
            Country = country;
            Cities = cities;
        }
    }
}
