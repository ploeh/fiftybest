using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class YearsModel(FiftyBestData data) : PageModel
{
    public ushort Year { get; private set; }
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
