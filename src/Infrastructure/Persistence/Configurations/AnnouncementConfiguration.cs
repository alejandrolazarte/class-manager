using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    private const int CategoryMaxLength = 20;
    private const string SinglePinnedIndexName = "IX_Announcements_TenantId_Pinned";
    private const string SinglePinnedFilter = $"[IsPinned] = 1 AND {SoftDeleteModelBuilderExtensions.NotDeletedFilter}";

    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(announcement => announcement.Id);
        builder.Property(announcement => announcement.Id).ValueGeneratedNever();
        builder.Property(announcement => announcement.Title).HasMaxLength(Announcement.TitleMaxLength).IsRequired();
        builder.Property(announcement => announcement.Body).HasMaxLength(Announcement.BodyMaxLength);
        builder.Property(announcement => announcement.Category).HasConversion<string>().HasMaxLength(CategoryMaxLength);
        builder.Ignore(announcement => announcement.ClassGroupIds);
        builder.Ignore(announcement => announcement.IsForEveryone);
        builder.Ignore(announcement => announcement.ImagesInOrder);
        builder.Ignore(announcement => announcement.ImageCount);
        builder.Ignore(announcement => announcement.MaximumImageCount);

        builder.HasOne<Business>().WithMany().HasForeignKey(announcement => announcement.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(announcement => new { announcement.TenantId, announcement.PublishedAt });
        builder.HasIndex(announcement => announcement.TenantId, SinglePinnedIndexName).IsUnique().HasFilter(SinglePinnedFilter);

        builder.HasMany(announcement => announcement.ClassGroups)
            .WithOne()
            .HasForeignKey(classGroup => classGroup.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(announcement => announcement.ClassGroups).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();

        builder.HasMany(announcement => announcement.Images)
            .WithOne()
            .HasForeignKey(image => image.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(announcement => announcement.Images).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();
    }
}
