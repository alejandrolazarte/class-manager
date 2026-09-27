using ClassManager.Security.Tokens;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Security.Hosting;

public sealed class SecurityOptions
{
    public const int DefaultPasswordMinLength = 10;
    public const int DefaultMaxFailedSignInAttempts = 5;

    public static readonly TimeSpan DefaultLockoutDuration = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan DefaultPasswordResetTokenLifetime = TimeSpan.FromHours(24);

    public string TenantClaimType { get; set; } = JwtOptions.DefaultTenantClaimType;

    public int PasswordMinLength { get; set; } = DefaultPasswordMinLength;

    public int MaxFailedSignInAttempts { get; set; } = DefaultMaxFailedSignInAttempts;

    public TimeSpan LockoutDuration { get; set; } = DefaultLockoutDuration;

    public TimeSpan PasswordResetTokenLifetime { get; set; } = DefaultPasswordResetTokenLifetime;

    /// <summary>
    /// How <see cref="Persistence.SecurityDbContext"/> connects to the database. Required: the application decides it,
    /// for example sharing the connection of its own DbContext so both can join one transaction.
    /// </summary>
    public Action<IServiceProvider, DbContextOptionsBuilder>? ConfigureDbContext { get; set; }
}
