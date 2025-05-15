namespace CoreFinance.Contracts.Utilities;

public static class DictionaryHelper
{
    public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        Dictionary<TKey, TValue> dictionaryToAdd)
        where TKey : notnull
    {
        foreach (var item in dictionaryToAdd)
        {
            source[item.Key] = item.Value;
        }

        return source;
    }
}