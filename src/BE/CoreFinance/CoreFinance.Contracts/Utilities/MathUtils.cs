namespace CoreFinance.Contracts.Utilities;

public static class MathUtils
{
    public static decimal? SumDecimal(
        params decimal?[]? values)
    {
        return values?.Where(value => value.HasValue)
            .Sum(value => value!.Value);
    }

    public static decimal NullToZero(
        this decimal? value)
    {
        return value ?? 0m;
    }

    public static int NullToZero(
        this int? value)
    {
        return value ?? 0;
    }
}