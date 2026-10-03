using ClassManager.Core.Domain.Images;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassPackConfiguration : IEntityTypeConfiguration<ClassPack>
{
    public void Configure(EntityTypeBuilder<ClassPack> builder)
    {
        builder.HasKey(classPack => classPack.Id);
        builder.Property(classPack => classPack.Id).ValueGeneratedNever();
        builder.Property(classPack => classPack.Name).HasMaxLength(ClassPack.NameMaxLength).IsRequired();
        builder.Property(classPack => classPack.Price).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Property(classPack => classPack.MaterialUrl).HasMaxLength(ClassPack.MaterialUrlMaxLength);
        builder.Property(classPack => classPack.ImageUrl).HasMaxLength(CatalogImage.UrlMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(classPack => classPack.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(classPack => new { classPack.TenantId, classPack.Name }).IsUnique();
        builder.Ignore(classPack => classPack.ClassGroupIds);

        builder.HasMany(classPack => classPack.ClassGroups)
            .WithOne()
            .HasForeignKey(classGroup => classGroup.ClassPackId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(classPack => classPack.ClassGroups).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();
    }
}
