using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Identity;
using MesEnterprise.Infrastructure.Persistence;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MesEnterprise.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly MesDbContext _dbContext;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(MesDbContext dbContext, ILogger<IdentityService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<User?> GetUserAsync(string userName, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.Include(u => u.Roles).ThenInclude(ur => ur.Role)
            .Include(u => u.Claims)
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<bool> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userName, cancellationToken);
        if (user is null || user.IsLocked)
        {
            return false;
        }

        return VerifyPassword(password, user.PasswordHash);
    }

    public async Task<IReadOnlyCollection<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role!.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role!.Permissions)
            .Select(rp => rp.Permission!.Code)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3)
        {
            return false;
        }

        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var hash = Convert.FromBase64String(parts[2]);

        var inputHash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterations, hash.Length);
        return hash.SequenceEqual(inputHash);
    }

    public static string HashPassword(string password, int iterations = 10000)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterations, 32);
        return string.Join('.', iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }
}
