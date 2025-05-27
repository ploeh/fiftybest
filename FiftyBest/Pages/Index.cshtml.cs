using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class IndexModel(FiftyBestData data) : PageModel
{
    public int Count { get; } = data.EntriesByYear.Count;
}
