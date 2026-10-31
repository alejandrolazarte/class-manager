using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassPackConfiguration : IEntityTypeConfiguration<ClassPack>
{
    private const string LibreClassesOnlyWhenRenewingConstraintName = "CK_ClassPacks_ClassCount_RenewsMonthly";

    public void Configure(EntityTypeBuilder<ClassPack> builder)
    {
        builder.HasKey(classPack => classPack.Id);
        builder.Property(classPack => classPack.Name).HasMaxLength(ClassPack.NameMaxLength).IsRequired();
        builder.Property(classPack => classPack.Description).HasMaxLength(ClassPack.DescriptionMaxLength);
        builder.Property(classPack => classPack.Price).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);

        builder.HasOne<Business>().WithMany().HasForeignKey(classPack => classPack.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(classPack => new { classPack.TenantId, classPack.Name }).IsUnique();
        builder.Ignore(classPack => classPack.ClassGroupIds);
        builder.Ignore(classPack => classPack.ImagesInOrder);
        builder.Ignore(classPack => classPack.ImageCount);
        builder.Ignore(classPack => classPack.MaximumImageCount);
        builder.Ignore(classPack => classPack.IsSoldOnce);
        builder.ToTable(table => table.HasCheckConstraint(
            LibreClassesOnlyWhenRenewingConstraintName,
            "[ClassCount] IS NOT NULL OR [RenewsMonthly] = 1"));

        builder.HasMany(classPack => classPack.ClassGroups)
            .WithOne()
            .HasForeignKey(classGroup => classGroup.ClassPackId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(classPack => classPack.ClassGroups).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();

        builder.HasMany(classPack => classPack.Images)
            .WithOne()
            .HasForeignKey(image => image.ClassPackId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(classPack => classPack.Images).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();
    }
}
