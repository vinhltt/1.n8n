using System.Text;
using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using Identity.Api.Middleware;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication (for legacy API compatibility)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured");

builder.Services.AddAuthentication("JWT")
.AddJwtBearer("JWT", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUser", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("sub"));

    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin"));

    options.AddPolicy("RequireUserOrAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.IsInRole("Admin") || 
                  context.User.HasClaim("sub", context.User.FindFirst("sub")?.Value ?? "")));
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Identity & Access API", 
        Version = "v1",
        Description = "Identity and Access Management API for PFM System"
    });

    // JWT Bearer token configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
                }
            },
            []
        }
    });
});

// CORS for SSO - Support multiple clients
builder.Services.AddCors(options =>
{
        // API access policy for external services
    options.AddPolicy("AllowAPI", policy =>
    {
        policy.WithOrigins(
                // Local development
                "http://localhost:3000", "https://localhost:3000",
                "http://localhost:3001", "https://localhost:3001",
                "http://localhost:5217", "https://localhost:7226", // Identity.Sso
                // Production domains
                "https://app.pfm.vn", "https://pfm.vn", "https://login.pfm.vn",
                // Mobile development (for WebView)
                "capacitor://localhost", "ionic://localhost",
                "http://localhost", "https://localhost"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity & Access API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Apply migrations automatically in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

// Global exception handling
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors("AllowAPI");

app.UseAuthentication();
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
