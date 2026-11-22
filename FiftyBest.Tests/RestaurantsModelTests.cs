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

    [Fact]
    public void ConsecutiveYearsFormSingleStreak()
    {
        var name = new RestaurantName("Noma");
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2010, [new FiftyBestEntry(1, name, "Copenhagen", "Denmark")] },
            { 2011, [new FiftyBestEntry(1, name, "Copenhagen", "Denmark")] },
            { 2012, [new FiftyBestEntry(1, name, "Copenhagen", "Denmark")] }
        });
        var sut = new RestaurantsModel(data);

        sut.OnGet("Denmark", "Copenhagen", "Noma");

        Assert.Equal(
            [[new Ranking(2010, 1), new Ranking(2011, 1), new Ranking(2012, 1)]],
            sut.Streaks);
    }

    [Fact]
    public void YearOffTheListSplitsStreaks()
    {
        var name = new RestaurantName("Noma");
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2015, [new FiftyBestEntry(3, name, "Copenhagen", "Denmark")] },
            { 2016, [new FiftyBestEntry(5, name, "Copenhagen", "Denmark")] },
            // Not on the list in 2017 and 2018
            { 2017, [] },
            { 2018, [] },
            { 2019, [new FiftyBestEntry(2, name, "Copenhagen", "Denmark")] }
        });
        var sut = new RestaurantsModel(data);

        sut.OnGet("Denmark", "Copenhagen", "Noma");

        Assert.Equal(
            [[new Ranking(2015, 3), new Ranking(2016, 5)], [new Ranking(2019, 2)]],
            sut.Streaks);
    }

    [Fact]
    public void GapYearDoesNotSplitStreak()
    {
        var name = new RestaurantName("Noma");
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2019, [new FiftyBestEntry(2, name, "Copenhagen", "Denmark")] },
            // No data for 2020
            { 2021, [new FiftyBestEntry(1, name, "Copenhagen", "Denmark")] }
        });
        var sut = new RestaurantsModel(data);

        sut.OnGet("Denmark", "Copenhagen", "Noma");

        Assert.Equal(
            [[new Ranking(2019, 2), new Ranking(2021, 1)]],
            sut.Streaks);
    }
}
