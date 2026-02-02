using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync(string returnUrl = "/")
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect(returnUrl);
    }
}
