using CoreFinance.Application.Mapper;
using CoreFinance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using CoreFinance.Contracts;
using CoreFinance.Contracts.Utilities;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;
        schema.Enum.Clear();
        Enum.GetNames(context.Type)
            .ToList()
            .ForEach(name =>
                schema.Enum.Add(
                    new OpenApiString(
                        $"{Convert.ToInt64(Enum.Parse(context.Type, name))} = {name}")));
    }
}

public static class GeneralServiceExtension
{
    public static void AddGeneralConfigurations(
        this WebApplicationBuilder builder,
        string policyName,
        CorsOptions corsOption
    )
    {
        // Add DbContext
        builder.Services.AddDbContext<CoreFinanceDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("CoreFinanceDb"),
                    _ => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true))
                .UseSnakeCaseNamingConvention());
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // FluentValidation

        // AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
                {
                    // Ví dụ: sử dụng camelCase cho tên thuộc tính
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

                    // Ví dụ: bỏ qua các thuộc tính có giá trị null khi serialize
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;

                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }
            );
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "App Api", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"Example Token: 'Bearer {Token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oath2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
            c.SchemaFilter<EnumSchemaFilter>();
        });

        builder.Services.AddCors(c =>
        {
            c.AddPolicy(policyName, options =>
            {
                options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                if (corsOption.AllowedOrigins.IsAllowedAll())
                    options.AllowAnyOrigin();
                else
                    options.WithOrigins(corsOption.AllowedOrigins);

                if (corsOption.AllowedMethods.IsAllowedAll())
                    options.AllowAnyMethod();
                else
                    options.WithMethods(corsOption.AllowedMethods);

                if (corsOption.ExposedHeaders.IsAllowedAll())
                    options.AllowAnyHeader();
                else
                    options.WithHeaders(corsOption.ExposedHeaders);
            });
        });
    }

    private static bool IsAllowedAll(this IReadOnlyCollection<string>? values)
    {
        return values == null || values.Count == 0 || values.Contains("*");
    }
}