using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class MemberPushSubscriptionConfiguration : IEntityTypeConfiguration<MemberPushSubscription>
{
    public void Configure(EntityTypeBuilder<MemberPushSubscription> builder)
    {
        builder.HasKey(subscription => subscription.Id);
        builder.Property(subscription => subscription.Id).ValueGeneratedNever();
        builder.Property(subscription => subscription.Endpoint).HasMaxLength(MemberPushSubscription.EndpointMaxLength).IsRequired();
        builder.Property(subscription => subscription.P256dh).HasMaxLength(MemberPushSubscription.KeyMaxLength).IsRequired();
        builder.Property(subscription => subscription.Auth).HasMaxLength(MemberPushSubscription.KeyMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(subscription => subscription.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(subscription => new { subscription.TenantId, subscription.Endpoint }).IsUnique();
        builder.HasIndex(subscription => new { subscription.TenantId, subscription.UserId });
    }
}
