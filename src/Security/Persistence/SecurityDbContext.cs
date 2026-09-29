using ClassManager.Security.Accounts;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClassManager.Security.Persistence;

public sealed class SecurityDbContext(DbContextOptions<SecurityDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public const string SchemaName = "identity";
    public const string MigrationsHistoryTableName = "__EFMigrationsHistory";

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public static void ConfigureSqlServer(SqlServerDbContextOptionsBuilder sqlServer) =>
        sqlServer.MigrationsHistoryTable(MigrationsHistoryTableName, SchemaName);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(SchemaName);

        builder.Entity<ApplicationUser>(user =>
            user.Property(applicationUser => applicationUser.FullName).HasMaxLength(ApplicationUser.FullNameMaxLength).IsRequired());

        builder.Entity<RefreshToken>(refreshToken =>
        {
            refreshToken.HasKey(token => token.Id);
            refreshToken.Property(token => token.Id).ValueGeneratedNever();
            refreshToken.Property(token => token.TokenHash).HasMaxLength(RefreshToken.TokenHashLength).IsFixedLength().IsRequired();
            refreshToken.Property(token => token.Kind).HasMaxLength(RefreshToken.KindMaxLength);
            refreshToken.HasIndex(token => token.TokenHash).IsUnique();
            refreshToken.HasIndex(token => token.UserId);
            refreshToken.HasOne<ApplicationUser>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PasswordResetToken>(passwordResetToken =>
        {
            passwordResetToken.HasKey(token => token.Id);
            passwordResetToken.Property(token => token.Id).ValueGeneratedNever();
            passwordResetToken.Property(token => token.TokenHash)
                .HasMaxLength(PasswordResetToken.TokenHashLength).IsFixedLength().IsRequired();
            passwordResetToken.HasIndex(token => token.TokenHash).IsUnique();
            passwordResetToken.HasIndex(token => token.UserId);
            passwordResetToken.HasOne<ApplicationUser>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
