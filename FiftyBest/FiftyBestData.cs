using System.Globalization;

namespace Ploeh.FiftyBest;

public sealed class FiftyBestData
{
    public IReadOnlyDictionary<ushort, IReadOnlyCollection<FiftyBestEntry>> EntriesByYear { get; }
    public IReadOnlyDictionary<(string City, string Name), (string Country, IReadOnlyCollection<Ranking> Rankings)> Restaurants { get; }

    public FiftyBestData(
        IReadOnlyDictionary<ushort, IReadOnlyCollection<FiftyBestEntry>> entriesByYear)
    {
        EntriesByYear = entriesByYear;

        var gs =
            from kvp in entriesByYear
            let year = kvp.Key
            from entry in kvp.Value
            let k = (entry.Country, entry.City, entry.Name)
            let r = new Ranking(year, entry.Rank)
            group r by k into g
            select KeyValuePair.Create(
                (g.Key.City, g.Key.Name),
                (g.Key.Country, g.OrderBy(t => t.Year).ReadOnly()));
        Restaurants = gs.ToDictionary();
    }

    public static FiftyBestData Parse(
        IReadOnlyCollection<(ushort Year, IReadOnlyCollection<string> Lines)> input)
    {
        var entriesByYear = input
            .Select(t => KeyValuePair.Create(
                t.Year,
                t.Lines.Select(FiftyBestEntry.Parse).ReadOnly()))
            .ToDictionary();
        return new FiftyBestData(entriesByYear);
    }

    public static FiftyBestData Load(string path)
    {
        var fileNames = Directory.GetFiles(path, "*.csv");
        var input = fileNames
            .Select(fileName => (
                ushort.Parse(
                    Path.GetFileNameWithoutExtension(fileName),
                    CultureInfo.InvariantCulture),
                File.ReadAllLines(fileName).ReadOnly()))
            .ReadOnly();
        return Parse(input);
    }
}
