namespace CoreFinance.Api.Infrastructures.Modules;

public static class EnvironmentVariableModule
{
    public static IServiceCollection AddConfigurationSettings(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        //var configuration = builder.Configuration;
        //services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        //services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        //services.Configure<ConnectionString>(configuration.GetSection("ConnectionStrings"));
        //services.Configure<CorsOptions>(configuration.GetSection("CorsOptions"));
        //services.Configure<GoogleCloudSetting>(configuration.GetSection("GoogleCloudSetting"));

        return services;
    }
}