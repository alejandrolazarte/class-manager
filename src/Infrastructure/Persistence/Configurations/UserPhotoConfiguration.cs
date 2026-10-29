using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class UserPhotoConfiguration : IEntityTypeConfiguration<UserPhoto>
{
    public void Configure(EntityTypeBuilder<UserPhoto> builder)
    {
        builder.HasKey(photo => photo.Id);
        builder.HasOne<User>().WithMany().HasForeignKey(photo => photo.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(photo => photo.UserId).IsUnique();
        builder.HasOne(photo => photo.Document).WithMany().HasForeignKey(photo => photo.DocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(photo => photo.DocumentId).IsUnique();
        builder.Navigation(photo => photo.Document).AutoInclude();

        builder.HasOne<Business>().WithMany().HasForeignKey(photo => photo.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(photo => photo.TenantId);
    }
}
