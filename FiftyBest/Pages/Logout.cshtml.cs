using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync(bool loggedOut = false)
    {
        if (!loggedOut)
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToPage(new { loggedOut = true });
        }

        return Page();
    }
}
