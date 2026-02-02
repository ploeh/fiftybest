using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ploeh.FiftyBest.Pages;
using System.Security.Claims;

namespace Ploeh.FiftyBest.Tests;

public sealed class LogoutModelTests
{
    [Fact]
    public async Task OnGetAsync()
    {
        var authService = new SpyAuthenticationService();
        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(authService);
        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };
        var sut = new LogoutModel
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        var result = await sut.OnGetAsync("/Countries/Denmark");

        var redirect = Assert.IsAssignableFrom<RedirectResult>(result);
        Assert.Equal("/Countries/Denmark", redirect.Url);
        Assert.Contains(
            new SpyAuthenticationService.SignOut(
                httpContext,
                "Cookies",
                null),
            authService.SignOuts);
    }

    private sealed class SpyAuthenticationService : IAuthenticationService
    {
        private readonly List<SignOut> signOuts = [];

        public IReadOnlyCollection<SignOut> SignOuts => signOuts;

        public Task<AuthenticateResult> AuthenticateAsync(
            HttpContext context,
            string? scheme)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public Task ChallengeAsync(
            HttpContext context,
            string? scheme,
            AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(
            HttpContext context,
            string? scheme,
            AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task SignInAsync(
            HttpContext context,
            string? scheme,
            ClaimsPrincipal principal,
            AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task SignOutAsync(
            HttpContext context,
            string? scheme,
            AuthenticationProperties? properties)
        {
            signOuts.Add(new SignOut(context, scheme, properties));
            return Task.CompletedTask;
        }

        public sealed record SignOut(
            HttpContext Context,
            string? Scheme,
            AuthenticationProperties? Properties);
    }
}
