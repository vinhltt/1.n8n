using System.Text.RegularExpressions;

namespace CoreFinance.Contracts.Utilities;

public static class StringHelper
{
    public static readonly string PascalCaseStringPattern = @"^_+";
    public static readonly string PascalCaseStringPatternReplace = @"([a-z0-9])(A-Z)";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        var startUnderScores = Regex.Match(value, PascalCaseStringPatternReplace);
        return startUnderScores + Regex.Replace(value, PascalCaseStringPatternReplace, "$1_$2").ToLower();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string Nullify(this string? value, string defaultValue = "", bool trim = true)
    {
        if (value == null)
        {
            return defaultValue;
        }
        if (trim)
        {
            value = value.Trim();
        }
        return value.Length == 0 ? defaultValue : value;
    }
    /// <summary>
    /// Trim start, Trim end and Trim middle space
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FullTrim(this string value)
    {
        return Regex.Replace(value, @"\s+", " ");
    }
}