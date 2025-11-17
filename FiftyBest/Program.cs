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
        options.ClientId =
            builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
        options.ClientSecret =
            builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
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
