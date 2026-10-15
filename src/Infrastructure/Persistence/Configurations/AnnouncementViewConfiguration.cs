using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AnnouncementViewConfiguration : IEntityTypeConfiguration<AnnouncementView>
{
    public void Configure(EntityTypeBuilder<AnnouncementView> builder)
    {
        builder.HasKey(view => new { view.AnnouncementId, view.ClientId });

        builder.HasOne<Announcement>().WithMany().HasForeignKey(view => view.AnnouncementId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Client>().WithMany().HasForeignKey(view => view.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Business>().WithMany().HasForeignKey(view => view.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(view => new { view.TenantId, view.ClientId });
    }
}
