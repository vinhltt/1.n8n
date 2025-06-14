using Identity.Application.Services.Authentication;
using Identity.Application.Services.Users;
using Identity.Contracts.Authentication;
using Identity.Contracts.Users;
using Identity.Contracts.Common;
using Identity.Sso.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Sso.Controllers;

/// <summary>
/// Authentication controller for SSO portal
/// Controller xác thực cho SSO portal
/// </summary>
[Route("auth")]
public class AuthController(IAuthService authService, IUserService userService) : Controller
{
    /// <summary>
    /// Display login page
    /// Hiển thị trang đăng nhập
    /// </summary>
    [HttpGet("login")]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        // Clear any existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }    /// <summary>
    /// Handle login form submission
    /// Xử lý form đăng nhập
    /// </summary>
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        Console.WriteLine($"[AUTH DEBUG] Login POST started - Username: {model.UsernameOrEmail}, ReturnUrl: {model.ReturnUrl}");
        
        if (!ModelState.IsValid)
        {
            Console.WriteLine("[AUTH DEBUG] ModelState is invalid:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"  - {error.ErrorMessage}");
            }
            return View(model);
        }

        Console.WriteLine("[AUTH DEBUG] ModelState is valid, proceeding with authentication...");        try
        {            var loginRequest = new LoginRequest(model.UsernameOrEmail, model.Password);

            var loginResponse = await authService.LoginAsync(loginRequest);
            Console.WriteLine($"[AUTH DEBUG] Authentication successful for user: {loginResponse.User.Email}");

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, loginResponse.User.Id.ToString()),
                new(ClaimTypes.Name, loginResponse.User.FullName),
                new(ClaimTypes.Email, loginResponse.User.Email),
                new("username", loginResponse.User.Username)
            };

            // Add roles to claims
            foreach (var role in loginResponse.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            Console.WriteLine($"[AUTH DEBUG] User signed in successfully. ReturnUrl: '{model.ReturnUrl}'");

            // Handle return URL with support for trusted external URLs
            // Xử lý return URL với hỗ trợ external URLs đáng tin cậy
            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                Console.WriteLine($"[AUTH DEBUG] Checking ReturnUrl: '{model.ReturnUrl}'");
                Console.WriteLine($"[AUTH DEBUG] IsLocalUrl: {Url.IsLocalUrl(model.ReturnUrl)}");
                Console.WriteLine($"[AUTH DEBUG] IsTrustedUrl: {IsTrustedUrl(model.ReturnUrl)}");
                
                if (Url.IsLocalUrl(model.ReturnUrl) || IsTrustedUrl(model.ReturnUrl))
                {
                    Console.WriteLine($"[AUTH DEBUG] ReturnUrl is trusted, redirecting to: {model.ReturnUrl}");
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    Console.WriteLine($"[AUTH DEBUG] ReturnUrl is NOT trusted, falling back to Dashboard");
                }
            }
            else
            {
                Console.WriteLine("[AUTH DEBUG] No ReturnUrl provided, redirecting to Dashboard");
            }

            Console.WriteLine("[AUTH DEBUG] Redirecting to Dashboard");
            return RedirectToAction("Dashboard", "Home");        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"[AUTH DEBUG] Authentication failed - Unauthorized: {ex.Message}");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH DEBUG] Authentication failed - Exception: {ex.Message}");
            Console.WriteLine($"[AUTH DEBUG] Exception StackTrace: {ex.StackTrace}");
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return View(model);
        }
    }

    /// <summary>
    /// Display registration page
    /// Hiển thị trang đăng ký
    /// </summary>
    [HttpGet("register")]
    public IActionResult Register(string? returnUrl = null)
    {
        var model = new RegisterViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    /// <summary>
    /// Handle registration form submission
    /// Xử lý form đăng ký
    /// </summary>
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {            var createUserRequest = new CreateUserRequest(
                model.Email,
                model.Username,
                model.FullName,
                model.FullName.Split(' ').FirstOrDefault() ?? "",
                model.FullName.Split(' ').LastOrDefault() ?? "",
                null,
                model.Password);

            await userService.CreateAsync(createUserRequest);// Auto-login after successful registration
            var loginRequest = new LoginRequest(model.Username, model.Password);

            var loginResponse = await authService.LoginAsync(loginRequest);

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, loginResponse.User.Id.ToString()),
                new(ClaimTypes.Name, loginResponse.User.FullName),
                new(ClaimTypes.Email, loginResponse.User.Email),
                new("username", loginResponse.User.Username)
            };

            // Add roles to claims
            foreach (var role in loginResponse.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    /// <summary>
    /// Handle logout
    /// Xử lý đăng xuất
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
                }
        
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Access denied page
    /// Trang từ chối truy cập
    /// </summary>
    [HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #region Private Helper Methods

    /// <summary>
    /// Check if URL is trusted for redirect
    /// Kiểm tra URL có đáng tin cậy cho redirect không
    /// </summary>
    private bool IsTrustedUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        try
        {
            var uri = new Uri(url);
            
            // Allow localhost URLs for development
            // Cho phép localhost URLs cho development
            if (uri.Host == "localhost" || uri.Host == "127.0.0.1")
            {
                // Allow common development ports
                // Cho phép các ports development thông dụng
                var allowedPorts = new[] { 3000, 3001, 8080, 5173, 4200 };
                return allowedPorts.Contains(uri.Port);
            }            // Add more trusted domains here as needed
            // Thêm các domains đáng tin cậy khác ở đây nếu cần
            var trustedHosts = new string[]
            {
                // Add production domains here
                // Thêm production domains ở đây
            };

            return trustedHosts.Any(host => string.Equals(host, uri.Host, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
