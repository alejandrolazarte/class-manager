using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(announcement => announcement.Id);
        builder.Property(announcement => announcement.Id).ValueGeneratedNever();
        builder.Property(announcement => announcement.Title).HasMaxLength(Announcement.TitleMaxLength).IsRequired();
        builder.Property(announcement => announcement.Body).HasMaxLength(Announcement.BodyMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(announcement => announcement.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(announcement => new { announcement.TenantId, announcement.PublishedAt });
    }
}
