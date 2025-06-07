using Identity.Contracts.Roles;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Application.Services.Roles;

/// <summary>
/// Service for managing role operations (EN)<br/>
/// Dịch vụ quản lý các thao tác vai trò (VI)
/// </summary>
/// <param name="roleRepository">
/// Repository for role data access (EN)<br/>
/// Repository để truy cập dữ liệu vai trò (VI)
/// </param>
/// <param name="userRoleRepository">
/// Repository for user-role relationship data access (EN)<br/>
/// Repository để truy cập dữ liệu quan hệ người dùng-vai trò (VI)
/// </param>
/// <param name="userRepository">
/// Repository for user data access (EN)<br/>
/// Repository để truy cập dữ liệu người dùng (VI)
/// </param>
public class RoleService(
    IRoleRepository roleRepository, 
    IUserRoleRepository userRoleRepository,
    IUserRepository userRepository) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<RoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        return role == null ? null : MapToRoleResponse(role);
    }

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(MapToRoleResponse);
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        // Check if role name already exists
        var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingRole != null)
        {
            throw new InvalidOperationException($"Role with name '{request.Name}' already exists");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Permissions = request.Permissions.ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _roleRepository.AddAsync(role, cancellationToken);
        return MapToRoleResponse(role);
    }    public async Task<RoleResponse> UpdateAsync(Guid roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found");
        }

        // Check if new name conflicts with existing role
        if (!string.IsNullOrEmpty(request.Name) && request.Name != role.Name)
        {
            var nameExists = await _roleRepository.IsNameExistsAsync(request.Name, roleId, cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException($"Role with name '{request.Name}' already exists");
            }
            role.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Description))
            role.Description = request.Description;

        if (request.Permissions != null)
            role.Permissions = request.Permissions.ToList();

        role.UpdatedAt = DateTime.UtcNow;
        await _roleRepository.UpdateAsync(role, cancellationToken);

        return MapToRoleResponse(role);
    }    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found");
        }

        // Check if role is assigned to any users
        var userRoles = await _userRoleRepository.GetByRoleIdAsync(roleId, cancellationToken);
        if (userRoles.Any())
        {
            throw new InvalidOperationException("Cannot delete role that is assigned to users");
        }        await _roleRepository.DeleteAsync(roleId, cancellationToken);
    }

    public async Task AssignRoleToUserAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Verify role exists
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found");
        }

        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        // Check if user already has this role
        var existingUserRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, cancellationToken);
        if (existingUserRole != null)
        {
            return; // User already has this role
        }        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = Guid.Empty // System assigned - would be set from auth context in real implementation
        };

        await _userRoleRepository.AddAsync(userRole, cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
    {
        var userRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, cancellationToken);
        if (userRole == null)
        {
            return; // User doesn't have this role
        }

        await _userRoleRepository.DeleteAsync(userRole.Id, cancellationToken);
    }    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userRoleRepository.GetByUserIdAsync(userId, cancellationToken);
        var permissions = new List<string>();
        
        foreach (var userRole in userRoles)
        {
            var role = await _roleRepository.GetByIdAsync(userRole.RoleId, cancellationToken);
            if (role != null)
            {
                permissions.AddRange(role.Permissions);
            }
        }
        
        return permissions.Distinct();
    }    public async Task<bool> IsNameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        if (excludeRoleId.HasValue)
        {
            return await _roleRepository.IsNameExistsAsync(name, excludeRoleId.Value, cancellationToken);
        }
        return await _roleRepository.IsNameExistsAsync(name, cancellationToken);
    }

    private static RoleResponse MapToRoleResponse(Role role)
    {
        return new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.Permissions,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }
}
