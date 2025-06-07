using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class ApiKeyRepository : BaseRepository<ApiKey, Guid>, IApiKeyRepository
{
    public ApiKeyRepository(IdentityDbContext context) : base(context)
    {
    }

    public async Task<ApiKey?> GetByKeyHashAsync(string keyHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ak => ak.User)
            .FirstOrDefaultAsync(ak => ak.KeyHash == keyHash, cancellationToken);
    }

    public async Task<IEnumerable<ApiKey>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ak => ak.UserId == userId)
            .OrderByDescending(ak => ak.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApiKey?> GetActiveKeyByHashAsync(string keyHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ak => ak.User)
            .FirstOrDefaultAsync(ak => ak.KeyHash == keyHash && 
                                      ak.Status == Domain.Enums.ApiKeyStatus.Active,
                                 cancellationToken);
    }

    public async Task UpdateLastUsedAsync(Guid apiKeyId, DateTime lastUsedAt, CancellationToken cancellationToken = default)
    {
        var apiKey = await _dbSet.FindAsync([apiKeyId], cancellationToken);
        if (apiKey != null)
        {
            apiKey.LastUsedAt = lastUsedAt;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task IncrementUsageCountAsync(Guid apiKeyId, CancellationToken cancellationToken = default)
    {
        var apiKey = await _dbSet.FindAsync([apiKeyId], cancellationToken);
        if (apiKey != null)
        {
            apiKey.UsageCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
