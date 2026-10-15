using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AnnouncementReplyConfiguration : IEntityTypeConfiguration<AnnouncementReply>
{
    public void Configure(EntityTypeBuilder<AnnouncementReply> builder)
    {
        builder.HasKey(reply => new { reply.AnnouncementId, reply.ClientId });

        builder.HasOne<Announcement>().WithMany().HasForeignKey(reply => reply.AnnouncementId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Client>().WithMany().HasForeignKey(reply => reply.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Business>().WithMany().HasForeignKey(reply => reply.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(reply => new { reply.TenantId, reply.ClientId });
    }
}
