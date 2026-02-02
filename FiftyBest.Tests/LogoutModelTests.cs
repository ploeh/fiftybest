using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ploeh.FiftyBest.Pages;

namespace Ploeh.FiftyBest.Tests;

public sealed class LogoutModelTests
{
    [Fact]
    public async Task OnGetAsync()
    {
        var authServiceMock = new Mock<IAuthenticationService>();

        var services = new ServiceCollection();
        services.AddSingleton(authServiceMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        var sut = new LogoutModel
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        var result = await sut.OnGetAsync();

        Assert.IsType<PageResult>(result);
        authServiceMock.Verify(
            x => x.SignOutAsync(
                httpContext,
                "Cookies",
                null),
            Times.Once);
    }
}
