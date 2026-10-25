using ClassManager.Security.Accounts;
using ClassManager.Security.Passkeys;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClassManager.Security.Persistence;

public sealed class SecurityDbContext(DbContextOptions<SecurityDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public const string SchemaName = "identity";
    public const string MigrationsHistoryTableName = "__EFMigrationsHistory";

    public static readonly Version IdentitySchemaVersion = IdentitySchemaVersions.Version3;

    private const string EmailIndexName = "EmailIndex";
    private const string NormalizedEmailIsNotNullFilter = "[NormalizedEmail] IS NOT NULL";

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public DbSet<EmailChangeToken> EmailChangeTokens => Set<EmailChangeToken>();

    public DbSet<PasskeyCeremony> PasskeyCeremonies => Set<PasskeyCeremony>();

    public static void ConfigureSqlServer(SqlServerDbContextOptionsBuilder sqlServer) =>
        sqlServer.MigrationsHistoryTable(MigrationsHistoryTableName, SchemaName);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(SchemaName);

        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(applicationUser => applicationUser.FullName).HasMaxLength(ApplicationUser.FullNameMaxLength).IsRequired();
            user.Property(applicationUser => applicationUser.PreferredLanguage)
                .HasMaxLength(ApplicationUser.PreferredLanguageMaxLength)
                .HasDefaultValue(ApplicationUser.DefaultPreferredLanguage)
                .IsRequired();
            user.HasIndex(applicationUser => applicationUser.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName(EmailIndexName)
                .HasFilter(NormalizedEmailIsNotNullFilter);
        });

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

        builder.Entity<EmailChangeToken>(emailChangeToken =>
        {
            emailChangeToken.HasKey(token => token.Id);
            emailChangeToken.Property(token => token.Id).ValueGeneratedNever();
            emailChangeToken.Property(token => token.NewEmail).HasMaxLength(EmailChangeToken.NewEmailMaxLength).IsRequired();
            emailChangeToken.Property(token => token.TokenHash)
                .HasMaxLength(EmailChangeToken.TokenHashLength).IsFixedLength().IsRequired();
            emailChangeToken.HasIndex(token => token.TokenHash).IsUnique();
            emailChangeToken.HasIndex(token => token.UserId);
            emailChangeToken.HasOne<ApplicationUser>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PasskeyCeremony>(passkeyCeremony =>
        {
            passkeyCeremony.HasKey(ceremony => ceremony.Id);
            passkeyCeremony.Property(ceremony => ceremony.Id).ValueGeneratedNever();
            passkeyCeremony.Property(ceremony => ceremony.CeremonyIdHash)
                .HasMaxLength(PasskeyCeremony.CeremonyIdHashLength).IsFixedLength().IsRequired();
            passkeyCeremony.Property(ceremony => ceremony.Kind)
                .HasConversion<string>().HasMaxLength(PasskeyCeremony.KindMaxLength).IsRequired();
            passkeyCeremony.HasIndex(ceremony => ceremony.CeremonyIdHash).IsUnique();
            passkeyCeremony.HasIndex(ceremony => ceremony.ExpiresAt);
            passkeyCeremony.HasOne<ApplicationUser>().WithMany().HasForeignKey(ceremony => ceremony.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
