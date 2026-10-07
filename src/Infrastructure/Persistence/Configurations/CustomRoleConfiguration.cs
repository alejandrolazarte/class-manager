using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class CustomRoleConfiguration : IEntityTypeConfiguration<CustomRole>
{
    private const char PermissionSeparator = ',';

    public void Configure(EntityTypeBuilder<CustomRole> builder)
    {
        builder.HasKey(role => role.Id);
        builder.Property(role => role.Id).ValueGeneratedNever();
        builder.Property(role => role.Name).HasMaxLength(CustomRole.NameMaxLength).IsRequired();
        builder.Property(role => role.CopiedFrom).HasMaxLength(CustomRole.CopiedFromMaxLength);
        builder.Property(role => role.Permissions)
            .HasConversion(
                permissions => string.Join(PermissionSeparator, permissions),
                storedPermissions => storedPermissions.Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries),
                new ValueComparer<IReadOnlyList<string>>(
                    (left, right) => left!.SequenceEqual(right!),
                    permissions => permissions.Aggregate(0, (hash, permission) => HashCode.Combine(hash, permission.GetHashCode(StringComparison.Ordinal))),
                    permissions => permissions.ToList()))
            .HasMaxLength(CustomRole.PermissionsMaxLength)
            .IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(role => role.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(role => new { role.TenantId, role.Name }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
    }
}
