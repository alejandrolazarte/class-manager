using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AnnouncementClassGroupConfiguration : IEntityTypeConfiguration<AnnouncementClassGroup>
{
    public void Configure(EntityTypeBuilder<AnnouncementClassGroup> builder)
    {
        builder.HasKey(announcementClassGroup => new { announcementClassGroup.AnnouncementId, announcementClassGroup.ClassGroupId });

        builder.HasOne<Business>().WithMany().HasForeignKey(announcementClassGroup => announcementClassGroup.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassGroup>().WithMany().HasForeignKey(announcementClassGroup => announcementClassGroup.ClassGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(announcementClassGroup => new { announcementClassGroup.TenantId, announcementClassGroup.ClassGroupId });
    }
}
