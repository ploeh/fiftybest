using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace Ploeh.FiftyBest.Pages;

public class RestaurantsModel(FiftyBestData data) : PageModel
{
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public RestaurantName? Name { get; private set; }
    public IReadOnlyCollection<string> Aliases { get; private set; } = [];
    public IReadOnlyCollection<Ranking> Rankings { get; private set; } = [];
    public IReadOnlyCollection<IReadOnlyCollection<Ranking>> Streaks { get; private set; } =
        [];
    public IReadOnlyCollection<RestaurantId> Restaurants { get; private set; } =
        [];

    public ushort FirstYear => Rankings.Min(r => r.Year);
    public ushort LastYear => Rankings.Max(r => r.Year);

    public IReadOnlyCollection<ushort> YearLabels
    {
        get
        {
            // Label every year when there are few, and every second or
            // third when there are many, so that the labels don't overlap.
            var step = (LastYear - FirstYear) / 10 + 1;
            var ys =
                from y in Enumerable.Range(FirstYear, LastYear - FirstYear + 1)
                where (y - FirstYear) % step == 0
                select (ushort)y;
            return ys.ReadOnly();
        }
    }

    public IReadOnlyCollection<byte> RankLabels => [1, 10, 20, 30, 40, 50];

    // The graph is 640 wide and 300 high. The plot area spans from Left to
    // Right horizontally, and from 20 to 265 vertically. All coordinates
    // are integers, because the decimal separator of fractional numbers
    // depends on the culture of the server.
    public int Left => 40;
    public int Right => 620;

    public IReadOnlyCollection<(Ranking First, Ranking Second)> Segments =>
        [.. Streaks.SelectMany(s => s.Zip(s.Skip(1)))];

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
            Aliases = data.Aliases.TryGetValue(
                rName.Name,
                out var aliases)
                ? aliases
                : [];
            Rankings = rankings;

            // A streak is a run of rankings from consecutive lists. Since
            // the rankings are ordered by year, the position of a ranking's
            // year among all years, less the position of the ranking
            // itself, stays the same for as long as a streak lasts.
            var years = data.EntriesByYear.Keys.Order().ToList();
            var ss =
                from t in rankings.Index()
                let r = t.Item
                let lead = years.IndexOf(r.Year) - t.Index
                group r by lead into g
                select g.ReadOnly();
            Streaks = ss.ReadOnly();
        }
    }

    public int X(ushort year)
    {
        // Divide the width evenly among the years, plus an empty year on
        // each side, so that no point sits on the edge. This also places a
        // single year in the middle, instead of dividing by zero.
        var slots = LastYear - FirstYear + 2;
        return Left + (year - FirstYear + 1) * (Right - Left) / slots;
    }

    public int Y(byte rank)
    {
        // Rank 1 is at the top, and each rank is 5 further down.
        return 20 + (rank - 1) * 5;
    }
}
