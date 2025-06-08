using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Enable runtime compilation for development

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Infrastructure (database, repositories, services)
builder.Services.AddInfrastructure(builder.Configuration);

// Authentication - Cookie-based for SSO
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.Name = "PFM.SSO.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Authorization
builder.Services.AddAuthorization();

// CORS for frontend applications
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSSO", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",  // Vue.js dev
                "http://localhost:5173",  // Vite dev
                "http://localhost:8080",  // Alternative Vue dev
                "https://localhost:3000",
                "https://localhost:5173",
                "https://localhost:8080"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Apply migrations automatically in development
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowSSO");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
