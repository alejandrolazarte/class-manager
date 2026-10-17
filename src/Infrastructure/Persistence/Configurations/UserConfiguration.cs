using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const string LinkedAccountFilter = "[AccountId] IS NOT NULL AND [IsDeleted] = 0";
    private const string ContactEmailFilter = "[Email] IS NOT NULL AND [IsDeleted] = 0";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.FullName).HasMaxLength(User.FullNameMaxLength).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(User.EmailMaxLength);
        builder.Property(user => user.PhoneNumber)
            .HasConversion(phoneNumber => phoneNumber!.Value, value => PhoneNumber.FromNormalized(value))
            .HasMaxLength(PhoneNumber.MaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(user => user.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(user => new { user.TenantId, user.AccountId }).IsUnique().HasFilter(LinkedAccountFilter);
        builder.HasIndex(user => new { user.TenantId, user.Email }).IsUnique().HasFilter(ContactEmailFilter);
        builder.HasIndex(user => new { user.TenantId, user.PhoneNumber });
        builder.HasIndex(user => user.AccountId);
    }
}
