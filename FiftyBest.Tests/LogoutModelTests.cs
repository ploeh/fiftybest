using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Ploeh.FiftyBest.Pages;

namespace Ploeh.FiftyBest.Tests;

public sealed class LogoutModelTests
{
    [Fact]
    public async Task OnGetAsyncReturnsRedirectToPageResultWhenNotLoggedOut()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProviderMock.Object
        };

        var sut = new LogoutModel
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        var result = await sut.OnGetAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirect.PageName);
        Assert.True(redirect.RouteValues?.ContainsKey("loggedOut"));
        Assert.Equal(true, redirect.RouteValues?["loggedOut"]);
    }

    [Fact]
    public async Task OnGetAsyncReturnsPageResultWhenLoggedOut()
    {
        var sut = new LogoutModel
        {
            PageContext = new PageContext { HttpContext = new DefaultHttpContext() }
        };

        var result = await sut.OnGetAsync(loggedOut: true);

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsyncCallsSignOut()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                "Cookies",
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProviderMock.Object
        };

        var sut = new LogoutModel
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        await sut.OnGetAsync();

        authServiceMock.Verify(
            x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                "Cookies",
                It.IsAny<AuthenticationProperties>()),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsyncDoesNotCallSignOutWhenLoggedOut()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                "Cookies",
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProviderMock.Object
        };

        var sut = new LogoutModel
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        await sut.OnGetAsync(loggedOut: true);

        authServiceMock.Verify(
            x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                "Cookies",
                It.IsAny<AuthenticationProperties>()),
            Times.Never);
    }
}
