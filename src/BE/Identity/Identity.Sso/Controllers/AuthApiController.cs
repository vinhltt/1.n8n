using Identity.Application.Services.Authentication;
using Identity.Application.Services.Users;
using Identity.Contracts.Authentication;
using Identity.Contracts.Users;
using Identity.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Sso.Controllers;

/// <summary>
/// API Controller for authentication operations (EN)
/// API Controller cho các thao tác xác thực (VI)
/// </summary>
[ApiController]
[Route("auth/api")]
public class AuthApiController(IAuthService authService, IUserService userService) : ControllerBase
{
    /// <summary>
    /// Traditional login with username/email and password (API endpoint)
    /// Đăng nhập truyền thống với username/email và mật khẩu (API endpoint)
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            var response = await authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Google OAuth2 login (API endpoint)
    /// Đăng nhập Google OAuth2 (API endpoint)
    /// </summary>
    [HttpPost("login/google")]
    public async Task<ActionResult<LoginResponse>> GoogleLoginAsync([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var response = await authService.GoogleLoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Register a new user (API endpoint)
    /// Đăng ký người dùng mới (API endpoint)
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> RegisterAsync([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await userService.CreateAsync(request);
            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User registered successfully",
                Data = user
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "An error occurred during registration"
            });
        }
    }

    /// <summary>
    /// Refresh authentication token (API endpoint)
    /// Làm mới token xác thực (API endpoint)
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Logout user (API endpoint)
    /// Đăng xuất người dùng (API endpoint)
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
    {
        try
        {
            await authService.LogoutAsync(request);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verify API key (Internal API endpoint)
    /// Xác thực API key (Internal API endpoint)
    /// </summary>
    [HttpPost("apikey/verify")]
    public async Task<ActionResult<ApiKeyVerificationResponse>> VerifyApiKeyAsync([FromBody] string apiKey)
    {
        try
        {
            var response = await authService.VerifyApiKeyAsync(apiKey);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
