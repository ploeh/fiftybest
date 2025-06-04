using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class YearsModel(FiftyBestData data) : PageModel
{
    public ushort Year { get; private set; }
    public ushort FirstYear => data.EntriesByYear.Keys.Min();

    public ushort? PreviousYear
    {
        get
        {
            var dict = Years.Skip(1).Zip(Years).ToDictionary();
            if (dict.TryGetValue(Year, out var previousYear))
                return previousYear;
            return null;
        }
    }

    public ushort? NextYear
    {
        get
        {
            var dict = Years.Zip(Years.Skip(1)).ToDictionary();
            if (dict.TryGetValue(Year, out var nextYear))
                return nextYear;
            return null;
        }
    }

    public ushort LastYear => data.EntriesByYear.Keys.Max();
    public IReadOnlyCollection<ushort> Years =>
        [.. data.EntriesByYear.Keys.OrderBy(y => y)];
    public IReadOnlyCollection<FiftyBestEntry> Entries { get; private set; } =
        [];

    public void OnGet(ushort? year)
    {
        Year = year ?? data.EntriesByYear.Keys.Max();
        Entries = data.EntriesByYear[Year];
    }
}