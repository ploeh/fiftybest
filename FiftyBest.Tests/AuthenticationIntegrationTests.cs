using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Ploeh.FiftyBest.Tests;

public sealed class AuthenticationIntegrationTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public AuthenticationIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Authentication:GitHub:ClientId"] =
                            "fake-test-client-id",
                        ["Authentication:GitHub:ClientSecret"] =
                            "fake-test-client-secret"
                    });
            });
        });
    }

    [Fact]
    public async Task LoginRedirectsToGitHubAuthenticationScheme()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/Login");

        AssertRedirectsToGitHub(response);
    }

    [Fact]
    public async Task LoginWithReturnUrlRedirectsToCorrectUrl()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/Login?returnUrl=/Years");

        AssertRedirectsToGitHub(response);
        Assert.Contains("redirect_uri=", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task LogoutReturnsLogoutPage()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/Logout");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Logged out", content);
        Assert.Contains(
            "You have been successfully logged out.",
            content);
    }

    private HttpClient CreateClient() =>
        factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

    private static void AssertRedirectsToGitHub(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location?.ToString();
        Assert.NotNull(location);
        Assert.StartsWith("https://github.com/login/oauth/authorize", location);
    }
}
