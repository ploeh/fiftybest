namespace Ploeh.FiftyBest;

public sealed record RestaurantName(string Name)
{
    public static RestaurantName FromUrlSegment(string urlSegment)
    {
        return new RestaurantName(urlSegment.Replace("__slash__", "/"));
    }

    public string ToUrlSegment()
    {
        return Name.Replace("/", "__slash__");
    }
};
