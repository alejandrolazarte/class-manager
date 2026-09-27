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

        builder.HasOne<Business>().WithMany().HasForeignKey(classPack => classPack.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(classPack => new { classPack.TenantId, classPack.Name }).IsUnique();
    }
}
