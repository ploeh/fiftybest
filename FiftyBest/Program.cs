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
        
        options.ClientId = 
            string.IsNullOrWhiteSpace(clientId) ? "dummy-client-id" : clientId;
        options.ClientSecret = 
            string.IsNullOrWhiteSpace(clientSecret) ? 
                "dummy-client-secret" : clientSecret;
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
