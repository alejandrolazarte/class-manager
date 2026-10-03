using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    private const int VisibilityMaxLength = 16;

    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(document => document.Id);
        builder.Property(document => document.Id).ValueGeneratedNever();
        builder.Property(document => document.Path).HasMaxLength(Document.PathMaxLength).IsRequired();
        builder.Property(document => document.ContentType).HasMaxLength(Document.ContentTypeMaxLength).IsRequired();
        builder.Property(document => document.Visibility).HasConversion<string>().HasMaxLength(VisibilityMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(document => document.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(document => document.TenantId);
    }
}
