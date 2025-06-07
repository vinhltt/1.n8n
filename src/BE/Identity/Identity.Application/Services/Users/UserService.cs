using Identity.Application.Common.Interfaces;
using Identity.Contracts.Users;
using Identity.Contracts.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Application.Services.Users;

/// <summary>
/// Service for managing user operations (EN)<br/>
/// Dịch vụ quản lý các thao tác người dùng (VI)
/// </summary>
/// <param name="userRepository">
/// Repository for user data access (EN)<br/>
/// Repository để truy cập dữ liệu người dùng (VI)
/// </param>
/// <param name="passwordHasher">
/// Service for password hashing and verification (EN)<br/>
/// Dịch vụ để băm và xác minh mật khẩu (VI)
/// </param>
public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user == null ? null : MapToUserResponse(user);
    }

    public async Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user == null ? null : MapToUserResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        // Validate email uniqueness if changed
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            if (await _userRepository.IsEmailExistsAsync(request.Email, userId, cancellationToken))
            {
                throw new InvalidOperationException("Email is already taken");
            }
            user.Email = request.Email;
        }

        // Validate username uniqueness if changed
        if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
        {
            if (await _userRepository.IsUsernameExistsAsync(request.Username, userId, cancellationToken))
            {
                throw new InvalidOperationException("Username is already taken");
            }
            user.Username = request.Username;
        }        // Update other fields - combine FirstName and LastName into FullName
        if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
        {
            var firstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : "";
            var lastName = !string.IsNullOrEmpty(request.LastName) ? request.LastName : "";
            user.FullName = $"{firstName} {lastName}".Trim();
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return MapToUserResponse(user);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        // For Google users who don't have a password yet, skip current password verification
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Current password is incorrect");
            }
        }

        // Validate new password confirmation
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new ArgumentException("New password and confirmation do not match");
        }

        // Hash new password
        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check if email is already taken
        if (await _userRepository.IsEmailExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email is already taken");
        }

        // Check if username is already taken
        if (await _userRepository.IsUsernameExistsAsync(request.Username, cancellationToken))
        {
            throw new InvalidOperationException("Username is already taken");
        }        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return MapToUserResponse(user);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        await _userRepository.DeleteAsync(userId, cancellationToken);
    }

    public async Task<PagedResponse<UserResponse>> GetPagedAsync(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetPagedAsync(page, pageSize, search, cancellationToken);
        var totalCount = await _userRepository.GetTotalCountAsync(search, cancellationToken);
        
        var userResponses = users.Select(MapToUserResponse).ToList();
        
        return new PagedResponse<UserResponse>
        {
            Items = userResponses,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<bool> ValidatePasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return false;
        }

        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<bool> IsEmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        if (excludeUserId.HasValue)
        {
            return await _userRepository.IsEmailExistsAsync(email, excludeUserId.Value, cancellationToken);
        }
        
        return await _userRepository.IsEmailExistsAsync(email, cancellationToken);
    }

    public async Task<bool> IsUsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        if (excludeUserId.HasValue)
        {
            return await _userRepository.IsUsernameExistsAsync(username, excludeUserId.Value, cancellationToken);
        }
        
        return await _userRepository.IsUsernameExistsAsync(username, cancellationToken);
    }    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            GoogleId = user.GoogleId,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
