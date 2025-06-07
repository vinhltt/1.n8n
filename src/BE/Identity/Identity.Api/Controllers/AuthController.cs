using Identity.Application.Services.Authentication;
using Identity.Application.Services.Users;
using Identity.Contracts.Authentication;
using Identity.Contracts.Users;
using Identity.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

/// <summary>
/// Controller for authentication operations including login, logout, and token management (EN)<br/>
/// Controller cho các thao tác xác thực bao gồm đăng nhập, đăng xuất và quản lý token (VI)
/// </summary>
/// <param name="authService">
/// Service for authentication operations (EN)<br/>
/// Dịch vụ cho các thao tác xác thực (VI)
/// </param>
/// <param name="userService">
/// Service for user operations (EN)<br/>
/// Dịch vụ cho các thao tác người dùng (VI)
/// </param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Traditional login with username/email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Register a new user (Temporary endpoint for testing)
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> RegisterAsync([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateAsync(request);
            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User registered successfully",
                Data = user
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Google OAuth2 login
    /// </summary>
    [HttpPost("login/google")]
    public async Task<ActionResult<LoginResponse>> GoogleLoginAsync([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var response = await _authService.GoogleLoginAsync(request);
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
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("token/refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
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
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
    {
        try
        {
            await _authService.LogoutAsync(request);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verify API key (internal use by API Gateway)
    /// </summary>
    [HttpPost("apikey/verify")]
    public async Task<ActionResult<ApiKeyVerificationResponse>> VerifyApiKeyAsync([FromBody] string apiKey)
    {
        try
        {
            var response = await _authService.VerifyApiKeyAsync(apiKey);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
