using Microsoft.OpenApi.Models;
using ExcelApi.Services;
using System.Reflection;
using System.Text;

// Đăng ký Code Page Provider để tránh lỗi encoding 1252 cho ExcelDataReader
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register ExcelProcessingService
builder.Services.AddScoped<IExcelProcessingService, ExcelProcessingService>();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with proper file upload support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Excel API",
        Version = "v1",
        Description = "API for processing Excel and CSV files."
    });

    // Enable XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.EnableAnnotations();

    // Map IFormFile to binary format for Swagger UI
    c.MapType<IFormFile>(() => new OpenApiSchema 
    { 
        Type = "string", 
        Format = "binary" 
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Excel API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
