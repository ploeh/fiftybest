using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class YearsModel(FiftyBestData data) : PageModel
{
    public ushort Year { get; set; }

    //public void OnGet()
    //{
    //    Year = data.EntriesByYear.Keys.Max();
    //}

    public void OnGet(ushort? year)
    {
        Year = year ?? data.EntriesByYear.Keys.Max();
    }
}
