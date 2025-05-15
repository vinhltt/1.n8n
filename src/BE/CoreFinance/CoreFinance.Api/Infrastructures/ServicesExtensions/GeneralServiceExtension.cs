using System.Text.Json.Serialization;
using CoreFinance.Application.Mapper;
using CoreFinance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        this WebApplicationBuilder builder
        )
    {

        // Add DbContext
        builder.Services.AddDbContext<CoreFinanceDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("CoreFinanceDb"))
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
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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
    }
}