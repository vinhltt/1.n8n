namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetOptions<T>(this IConfiguration configuration, string sectionName)
        where T : new()
    {
        return configuration.GetSection(sectionName).Get<T>();
    }
}