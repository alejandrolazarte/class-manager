using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassGroupMaterialFileConfiguration : IEntityTypeConfiguration<ClassGroupMaterialFile>
{
    public void Configure(EntityTypeBuilder<ClassGroupMaterialFile> builder)
    {
        builder.HasKey(materialFile => new { materialFile.ClassGroupId, materialFile.DocumentId });
        builder.HasOne(materialFile => materialFile.Document).WithMany().HasForeignKey(materialFile => materialFile.DocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(materialFile => materialFile.DocumentId).IsUnique();
        builder.Navigation(materialFile => materialFile.Document).AutoInclude();

        builder.HasOne<Business>().WithMany().HasForeignKey(materialFile => materialFile.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(materialFile => materialFile.TenantId);
    }
}
