using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Ploeh.FiftyBest.Pages;

namespace Ploeh.FiftyBest.Tests;

public sealed class LoginModelTests
{
    [Fact]
    public void OnGetReturnsChallengeResult()
    {
        var sut = new LoginModel();

        var result = sut.OnGet();

        Assert.IsType<ChallengeResult>(result);
    }

    [Fact]
    public void OnGetUsesGitHubScheme()
    {
        var sut = new LoginModel();

        var result = sut.OnGet();

        var challenge = Assert.IsType<ChallengeResult>(result);
        Assert.Contains("GitHub", challenge.AuthenticationSchemes);
    }

    [Fact]
    public void OnGetRedirectsToRootByDefault()
    {
        var sut = new LoginModel();

        var result = sut.OnGet();

        var challenge = Assert.IsType<ChallengeResult>(result);
        Assert.Equal("/", challenge.Properties?.RedirectUri);
    }

    [Fact]
    public void OnGetRedirectsToReturnUrl()
    {
        var sut = new LoginModel();

        var result = sut.OnGet("/Years");

        var challenge = Assert.IsType<ChallengeResult>(result);
        Assert.Equal("/Years", challenge.Properties?.RedirectUri);
    }
}
