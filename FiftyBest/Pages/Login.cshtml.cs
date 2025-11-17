using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ploeh.FiftyBest.Pages;

public class LoginModel : PageModel
{
    public IActionResult OnGet(string returnUrl = "/")
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = returnUrl },
            "GitHub");
    }
}
