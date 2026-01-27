using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public async Task LogoutRemovesAuthenticationAndUpdatesMenu()
    {
        // This test demonstrates the bug described in issue #16:
        // After logout, the menu should show "Login" instead of "Logout",
        // but currently it still shows the user as logged in.
        
        var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        // Step 1: Sign in using the test page
        var signinResponse = await client.GetAsync("/TestSignIn?userName=TestUser");
        Assert.Equal(HttpStatusCode.OK, signinResponse.StatusCode);

        // Step 2: Verify we're logged in by checking the home page
        var homeResponse = await client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();
        Assert.Contains("Hello, TestUser", homeContent);
        Assert.Contains("Logout</a>", homeContent);
        Assert.DoesNotContain("Login</a>", homeContent);

        // Step 3: Call logout
        var logoutResponse = await client.GetAsync("/Logout");
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        // Step 4: Verify the logout page shows correct authentication status
        var logoutContent = await logoutResponse.Content.ReadAsStringAsync();
        Assert.Contains("Logged out", logoutContent);
        Assert.Contains(
            "You have been successfully logged out.",
            logoutContent);
        // The menu should show "Login" and not show "Hello, TestUser"
        Assert.Contains("Login</a>", logoutContent);
        Assert.DoesNotContain("Hello, TestUser", logoutContent);

        // Step 5: Verify subsequent requests show unauthenticated state
        var afterLogoutResponse = await client.GetAsync("/");
        var afterLogoutContent =
            await afterLogoutResponse.Content.ReadAsStringAsync();
        Assert.Contains("Login</a>", afterLogoutContent);
        Assert.DoesNotContain("Hello, TestUser", afterLogoutContent);
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
