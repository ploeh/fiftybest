namespace Ploeh.FiftyBest;

public sealed record Restaurant(
    string Name,
    IReadOnlyCollection<Ranking> Rankings);