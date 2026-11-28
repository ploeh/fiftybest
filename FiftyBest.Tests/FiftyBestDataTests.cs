using Ploeh.FiftyBest;

namespace Ploeh.FiftyBest.Tests;

public sealed class FiftyBestDataTests
{
    [Fact]
    public void AliasedNamesFormSingleRestaurant()
    {
        var actual = FiftyBestData.Parse(
            [
                (2013, ["12,Frantzén/Lindeberg,Stockholm,Sweden"]),
                (2019, ["16,Frantzén,Stockholm,Sweden"])
            ],
            ["Frantzén,Frantzén/Lindeberg,Restaurant Frantzén"]);

        var kvp = Assert.Single(actual.Restaurants);
        Assert.Equal("Frantzén", kvp.Key.Name.Name);
        Assert.Equal(
            [new Ranking(2013, 12), new Ranking(2019, 16)],
            kvp.Value);
    }

    [Fact]
    public void LoadTakesAliasesFileIntoAccount()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            Path.GetRandomFileName());
        Directory.CreateDirectory(path);
        try
        {
            File.WriteAllText(
                Path.Combine(path, "2002.csv"),
                "40,Michel Bras,Laguiole,France");
            File.WriteAllText(
                Path.Combine(path, "2011.csv"),
                "30,Bras Restaurant,Laguiole, France");
            File.WriteAllText(
                Path.Combine(path, "aliases.txt"),
                """
                Frantzén,Frantzén/Lindeberg,Restaurant Frantzén
                Bras,Michel Bras,Bras Restaurant

                """);

            var actual = FiftyBestData.Load(path);

            var kvp = Assert.Single(actual.Restaurants);
            Assert.Equal("Bras", kvp.Key.Name.Name);
            Assert.Equal(
                [new Ranking(2002, 40), new Ranking(2011, 30)],
                kvp.Value);
        }
        finally
        {
            Directory.Delete(path, true);
        }
    }
}
