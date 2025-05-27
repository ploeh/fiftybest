using System.Globalization;

namespace Ploeh.FiftyBest;

public sealed class FiftyBestData(
    IReadOnlyDictionary<ushort, IReadOnlyCollection<FiftyBestEntry>> entriesByYear)
{
    public IReadOnlyDictionary<ushort, IReadOnlyCollection<FiftyBestEntry>> EntriesByYear { get; } = entriesByYear;

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
