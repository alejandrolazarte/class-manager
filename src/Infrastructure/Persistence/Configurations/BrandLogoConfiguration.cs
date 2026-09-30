using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class BrandLogoConfiguration : IEntityTypeConfiguration<BrandLogo>
{
    public void Configure(EntityTypeBuilder<BrandLogo> builder)
    {
        builder.HasKey(logo => logo.Id);
        builder.Property(logo => logo.Id).ValueGeneratedNever();
        builder.Property(logo => logo.Content).IsRequired();
        builder.Property(logo => logo.ContentType).HasMaxLength(BrandLogo.ContentTypeMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(logo => logo.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(logo => logo.TenantId).IsUnique();
    }
}
