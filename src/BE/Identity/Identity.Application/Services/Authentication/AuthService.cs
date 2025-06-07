using Identity.Application.Common.Interfaces;
using Identity.Contracts.Authentication;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Application.Services.Authentication;

/// <summary>
/// Service for authentication operations including login, token management, and API key validation (EN)<br/>
/// Dịch vụ cho các thao tác xác thực bao gồm đăng nhập, quản lý token và xác thực khóa API (VI)
/// </summary>
/// <param name="userRepository">
/// Repository for user data access (EN)<br/>
/// Repository để truy cập dữ liệu người dùng (VI)
/// </param>
/// <param name="refreshTokenRepository">
/// Repository for refresh token data access (EN)<br/>
/// Repository để truy cập dữ liệu refresh token (VI)
/// </param>
/// <param name="passwordHasher">
/// Service for password hashing and verification (EN)<br/>
/// Dịch vụ để băm và xác minh mật khẩu (VI)
/// </param>
/// <param name="jwtTokenService">
/// Service for JWT token generation and validation (EN)<br/>
/// Dịch vụ để tạo và xác thực JWT token (VI)
/// </param>
/// <param name="googleAuthService">
/// Service for Google OAuth authentication (EN)<br/>
/// Dịch vụ cho xác thực Google OAuth (VI)
/// </param>
/// <param name="apiKeyRepository">
/// Repository for API key data access (EN)<br/>
/// Repository để truy cập dữ liệu khóa API (VI)
/// </param>
/// <param name="apiKeyHasher">
/// Service for API key hashing and verification (EN)<br/>
/// Dịch vụ để băm và xác minh khóa API (VI)
/// </param>
public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IGoogleAuthService googleAuthService,
    IApiKeyRepository apiKeyRepository,
    IApiKeyHasher apiKeyHasher) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IGoogleAuthService _googleAuthService = googleAuthService;
    private readonly IApiKeyRepository _apiKeyRepository = apiKeyRepository;
    private readonly IApiKeyHasher _apiKeyHasher = apiKeyHasher;

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail, cancellationToken);
        
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        // Update last login
        await _userRepository.UpdateLastLoginAsync(user.Id, DateTime.UtcNow, cancellationToken);

        return await GenerateLoginResponseAsync(user, cancellationToken);
    }

    public async Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        var googleUser = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);

        var user = await _userRepository.GetByGoogleIdAsync(googleUser.GoogleId, cancellationToken);
        
        if (user == null)
        {
            // Check if user exists with same email
            user = await _userRepository.GetByEmailAsync(googleUser.Email, cancellationToken);
            
            if (user != null)
            {
                // Link Google account to existing user
                user.GoogleId = googleUser.GoogleId;
                user.AvatarUrl = googleUser.AvatarUrl;
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                // Create new user
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = googleUser.Email,
                    Username = googleUser.Email, // Use email as username for Google users
                    FullName = googleUser.FullName,
                    AvatarUrl = googleUser.AvatarUrl,
                    GoogleId = googleUser.GoogleId,
                    IsActive = true,
                    PasswordHash = string.Empty // Google users don't have password
                };
                
                await _userRepository.AddAsync(user, cancellationToken);
            }
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        // Update last login
        await _userRepository.UpdateLastLoginAsync(user.Id, DateTime.UtcNow, cancellationToken);

        return await GenerateLoginResponseAsync(user, cancellationToken);
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        
        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or disabled");
        }

        // Revoke old refresh token
        await _refreshTokenRepository.RevokeTokenAsync(request.RefreshToken, "system", cancellationToken);

        // Generate new tokens
        var userProfile = new UserProfile(user.Id, user.Email, user.Username, user.FullName, user.AvatarUrl, []);
        var newAccessToken = _jwtTokenService.GenerateAccessToken(userProfile);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store new refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.Add(_jwtTokenService.RefreshTokenLifetime)
        };
        
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        return new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.Add(_jwtTokenService.AccessTokenLifetime));
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeTokenAsync(request.RefreshToken, "user", cancellationToken);
    }

    public async Task<ApiKeyVerificationResponse> VerifyApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        var hashedKey = _apiKeyHasher.HashApiKey(apiKey);
        var apiKeyEntity = await _apiKeyRepository.GetActiveKeyByHashAsync(hashedKey, cancellationToken);

        if (apiKeyEntity == null)
        {
            return new ApiKeyVerificationResponse(false, null, [], null);
        }

        // Check if key is expired
        if (apiKeyEntity.ExpiresAt.HasValue && apiKeyEntity.ExpiresAt <= DateTime.UtcNow)
        {
            return new ApiKeyVerificationResponse(false, null, [], null);
        }

        // Update usage
        await _apiKeyRepository.UpdateLastUsedAsync(apiKeyEntity.Id, DateTime.UtcNow, cancellationToken);
        await _apiKeyRepository.IncrementUsageCountAsync(apiKeyEntity.Id, cancellationToken);

        var user = await _userRepository.GetByIdAsync(apiKeyEntity.UserId, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return new ApiKeyVerificationResponse(false, null, [], null);
        }

        var userProfile = new UserProfile(user.Id, user.Email, user.Username, user.FullName, user.AvatarUrl, []);

        return new ApiKeyVerificationResponse(true, user.Id, apiKeyEntity.Scopes, userProfile);
    }

    private async Task<LoginResponse> GenerateLoginResponseAsync(User user, CancellationToken cancellationToken)
    {
        var userProfile = new UserProfile(user.Id, user.Email, user.Username, user.FullName, user.AvatarUrl, []);
        
        var accessToken = _jwtTokenService.GenerateAccessToken(userProfile);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.Add(_jwtTokenService.RefreshTokenLifetime)
        };
        
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.Add(_jwtTokenService.AccessTokenLifetime),
            userProfile);
    }
}
