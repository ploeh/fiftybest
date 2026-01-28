using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        
        // Create a factory with a test authentication handler that
        // automatically authenticates requests
        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Add a test authentication handler that signs in users
                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = 
                            TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = 
                            TestAuthHandler.SchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.SchemeName,
                        options => { });
            });
        });

        var client = testFactory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        // Step 1: Verify we're logged in by checking the home page
        // The TestAuthHandler automatically authenticates all requests
        var homeResponse = await client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();
        Assert.Contains("Hello, TestUser", homeContent);
        Assert.Contains("Logout</a>", homeContent);
        Assert.DoesNotContain("Login</a>", homeContent);

        // Step 2: Call logout
        var logoutResponse = await client.GetAsync("/Logout");
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        // Step 3: Verify the logout page shows correct authentication status
        var logoutContent = await logoutResponse.Content.ReadAsStringAsync();
        Assert.Contains("Logged out", logoutContent);
        Assert.Contains(
            "You have been successfully logged out.",
            logoutContent);
        // The menu should show "Login" and not show "Hello, TestUser"
        Assert.Contains("Login</a>", logoutContent);
        Assert.DoesNotContain("Hello, TestUser", logoutContent);

        // Step 4: Verify subsequent requests show unauthenticated state
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

    private sealed class TestAuthHandler : AuthenticationHandler<
        AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestScheme";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            };
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
