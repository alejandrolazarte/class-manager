using ClassManager.Core.Domain.OnlinePayments;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class OnlinePaymentAccountConfiguration : IEntityTypeConfiguration<OnlinePaymentAccount>
{
    public void Configure(EntityTypeBuilder<OnlinePaymentAccount> builder)
    {
        builder.HasKey(account => account.Id);
        builder.Property(account => account.ExternalAccountId).HasMaxLength(OnlinePaymentAccount.ExternalAccountIdMaxLength).IsRequired();
        builder.Ignore(account => account.Status);
        builder.HasIndex(account => account.ExternalAccountId).IsUnique();
        builder.HasIndex(account => account.OrganizationId);
        builder.HasIndex(account => account.CreatedForBusinessId);
        builder.HasOne<Organization>().WithMany().HasForeignKey(account => account.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Business>().WithMany().HasForeignKey(account => account.CreatedForBusinessId).OnDelete(DeleteBehavior.Restrict);
    }
}
