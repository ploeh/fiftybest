using System.Globalization;

namespace Ploeh.FiftyBest;

public sealed class FiftyBestData
{
    public IReadOnlyDictionary<ushort, IReadOnlyCollection<FiftyBestEntry>> EntriesByYear { get; }
    public IReadOnlyDictionary<RestaurantId, IReadOnlyCollection<Ranking>> Restaurants { get; }
    public IReadOnlyDictionary<CityId, IReadOnlyCollection<Restaurant>> Cities { get; }
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Countries { get; }
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Aliases { get; }

    public FiftyBestData(
        IReadOnlyDictionary<ushort, IReadOnlyCollection<FiftyBestEntry>> entriesByYear,
        IReadOnlyDictionary<string, IReadOnlyCollection<string>>? aliases = null)
    {
        EntriesByYear = entriesByYear;
        Aliases = aliases ?? new Dictionary<string, IReadOnlyCollection<string>>();

        var rs =
            from kvp in entriesByYear
            let year = kvp.Key
            from entry in kvp.Value
            let k = new RestaurantId(
                new CityId(entry.Country, entry.City),
                entry.Name)
            let r = new Ranking(year, entry.Rank)
            group r by k into g
            select KeyValuePair.Create(
                g.Key,
                g.OrderBy(t => t.Year).ReadOnly());
        Restaurants = rs.ToDictionary();

        var cts =
            from kvp in Restaurants
            from ranking in kvp.Value
            let k = kvp.Key.City
            let v = new Restaurant(kvp.Key.Name, kvp.Value)
            group v by k into g
            orderby g.Key.Name
            select KeyValuePair.Create(
                g.Key,
                g.OrderBy(r => r.Name.Name).ReadOnly());
        Cities = cts.ToDictionary();

        var cos =
            from k in Cities.Keys.Distinct()
            group k.Name by k.Country into g
            orderby g.Key
            select KeyValuePair.Create(g.Key, g.Order().ReadOnly());
        Countries = cos.ToDictionary();
    }

    public static FiftyBestData Parse(
        IReadOnlyCollection<(ushort Year, IReadOnlyCollection<string> Lines)>
            input,
        IReadOnlyCollection<string>? aliases = null)
    {
        var canonicalNames = CanonicalNames(aliases);
        var entriesByYear = input
            .Select(t => KeyValuePair.Create(
                t.Year,
                t.Lines
                    .Select(FiftyBestEntry.Parse)
                    .Select(e => canonicalNames.TryGetValue(
                        e.Name.Name,
                        out var canonical)
                        ? e with { Name = new RestaurantName(canonical), ListedAs = e.Name }
                        : e)
                    .ReadOnly()))
            .ToDictionary();
        return new FiftyBestData(entriesByYear, AliasesByCanonical(aliases));
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
        var aliasPath = Path.Combine(path, "aliases.txt");
        IReadOnlyCollection<string> aliases =
            File.Exists(aliasPath)
                ? File.ReadAllLines(aliasPath)
                : [];
        return Parse(input, aliases);
    }

    private static Dictionary<string, string> CanonicalNames(
        IReadOnlyCollection<string>? aliases)
    {
        if (aliases is null || aliases.Count == 0)
            return [];
        return aliases
            .Select(line => line.Split(
                ',',
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries))
            .Where(parts => parts.Length > 1)
            .SelectMany(parts => parts
                .Skip(1)
                .Select(alias => (alias, Canonical: parts[0])))
            .ToDictionary(t => t.alias, t => t.Canonical);
    }

    private static Dictionary<string, IReadOnlyCollection<string>>
        AliasesByCanonical(IReadOnlyCollection<string>? aliases)
    {
        if (aliases is null || aliases.Count == 0)
            return [];
        return aliases
            .Select(line => line.Split(
                ',',
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries))
            .Where(parts => parts.Length > 1)
            .GroupBy(parts => parts[0])
            .ToDictionary(
                g => g.Key,
                g => g.SelectMany(parts => parts.Skip(1)).ReadOnly());
    }
}
