using Ploeh.FiftyBest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton(FiftyBestData.Load("Data"));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "GitHub";
    })
    .AddCookie("Cookies")
    .AddGitHub(options =>
    {
        var clientId =
            builder.Configuration["Authentication:GitHub:ClientId"];
        var clientSecret =
            builder.Configuration["Authentication:GitHub:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException(
                "GitHub ClientId is not configured.");
        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new InvalidOperationException(
                "GitHub ClientSecret is not configured.");

        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
