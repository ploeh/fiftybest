using Ploeh.FiftyBest.Pages;

namespace Ploeh.FiftyBest.Tests;
public sealed class RestaurantsModelTests
{
    [Fact]
    public void RestaurantWithSlashInName()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2013, [
                new FiftyBestEntry(
                    12,
                    new RestaurantName("Frantzén/Lindeberg"),
                    "Stockholm",
                    "Sweden")] }
        });
        var sut = new RestaurantsModel(data);

        // Notice that the restaurant name is URL-encoded
        sut.OnGet("Sweden", "Stockholm", "Frantzén__slash__Lindeberg");

        Assert.Equal("Frantzén/Lindeberg", sut.Name?.Name);
    }
}
