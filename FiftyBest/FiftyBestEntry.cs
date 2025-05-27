using System.Globalization;

namespace Ploeh.FiftyBest;

public sealed record FiftyBestEntry(
    byte Rank,
    string Name,
    string City,
    string Country)
{
    public static FiftyBestEntry Parse(string line)
    {
        var parts = line.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 4)
            throw new FormatException($"""Input must be a single line with exactly four parts separated by commas, but was: "{line}".""");
        return new FiftyBestEntry(
            byte.Parse(parts[0], CultureInfo.InvariantCulture),
            parts[1],
            parts[2],
            parts[3]);
    }
}
