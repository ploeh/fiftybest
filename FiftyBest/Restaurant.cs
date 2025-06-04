namespace Ploeh.FiftyBest;

public sealed record Restaurant(
    RestaurantName Name,
    IReadOnlyCollection<Ranking> Rankings);