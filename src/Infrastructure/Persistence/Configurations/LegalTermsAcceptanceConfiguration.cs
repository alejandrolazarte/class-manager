using ClassManager.Core.Domain.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class LegalTermsAcceptanceConfiguration : IEntityTypeConfiguration<LegalTermsAcceptance>
{
    public void Configure(EntityTypeBuilder<LegalTermsAcceptance> builder)
    {
        builder.HasKey(acceptance => acceptance.Id);
        builder.Property(acceptance => acceptance.Id).ValueGeneratedNever();
        builder.Property(acceptance => acceptance.Version).HasMaxLength(LegalTerms.VersionMaxLength).IsRequired();
        builder.HasIndex(acceptance => new { acceptance.UserId, acceptance.Version }).IsUnique();
    }
}
