namespace Ploeh.FiftyBest;

public static class ReadOnlyEnvy
{
    public static IReadOnlyCollection<T> ReadOnly<T>(this IEnumerable<T> source)
    {
        if (source is IReadOnlyCollection<T> readOnlyCollection)
            return readOnlyCollection;
        return source.ToList().AsReadOnly();
    }
}
