using CoreFinance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CoreFinance.Api.Infrastructures.ServicesExtensions;
using Serilog;

async Task CreateDbIfNotExistsAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<CoreFinanceDbContext>();
        await context.Database.MigrateAsync();
        //var dbInitializer = services.GetService<DbInitializer>();
        //if (dbInitializer == null)
        //{
        //    logger.LogError("dbInitializer is null");
        //    return;
        //}

        //await dbInitializer.Seed();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
        true, true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.AddGeneralConfigurations();
builder.Services.AddInjectedServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreFinance Api v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

await CreateDbIfNotExistsAsync(app);


try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
