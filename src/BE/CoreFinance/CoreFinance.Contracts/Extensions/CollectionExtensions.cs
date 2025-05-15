namespace CoreFinance.Contracts.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    /// Checks if the provided collection is null or empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        if (source is IEnumerable<string>)
        {
            return string.IsNullOrWhiteSpace(source as string);
        }

        return source == null || !source.Any();
    }
}