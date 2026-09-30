using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.HasKey(subscription => subscription.Id);
        builder.Property(subscription => subscription.Id).ValueGeneratedNever();
        builder.Property(subscription => subscription.Endpoint).HasMaxLength(PushSubscription.EndpointMaxLength).IsRequired();
        builder.Property(subscription => subscription.P256dh).HasMaxLength(PushSubscription.KeyMaxLength).IsRequired();
        builder.Property(subscription => subscription.Auth).HasMaxLength(PushSubscription.KeyMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(subscription => subscription.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(subscription => subscription.ClientId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(subscription => new { subscription.TenantId, subscription.Endpoint }).IsUnique();
        builder.HasIndex(subscription => new { subscription.TenantId, subscription.ClientId });
    }
}
