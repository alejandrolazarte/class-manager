using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class TeamNotificationConfiguration : IEntityTypeConfiguration<TeamNotification>
{
    public void Configure(EntityTypeBuilder<TeamNotification> builder)
    {
        builder.HasKey(notification => notification.Id);
        builder.Property(notification => notification.Id).ValueGeneratedNever();
        builder.Property(notification => notification.Title).HasMaxLength(TeamNotification.TitleMaxLength).IsRequired();
        builder.Property(notification => notification.Body).HasMaxLength(TeamNotification.BodyMaxLength).IsRequired();
        builder.Property(notification => notification.Url).HasMaxLength(TeamNotification.UrlMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(notification => notification.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(notification => new { notification.TenantId, notification.RecipientUserId, notification.CreatedAt });
    }
}
