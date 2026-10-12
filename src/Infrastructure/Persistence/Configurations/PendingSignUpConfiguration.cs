using ClassManager.Core.Domain.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class PendingSignUpConfiguration : IEntityTypeConfiguration<PendingSignUp>
{
    public void Configure(EntityTypeBuilder<PendingSignUp> builder)
    {
        builder.HasKey(pendingSignUp => pendingSignUp.Id);
        builder.Property(pendingSignUp => pendingSignUp.Id).ValueGeneratedNever();
        builder.Property(pendingSignUp => pendingSignUp.Email).HasMaxLength(OwnerAccount.EmailMaxLength).IsRequired();
        builder.Property(pendingSignUp => pendingSignUp.OwnerFullName).HasMaxLength(OwnerAccount.FullNameMaxLength).IsRequired();
        builder.Property(pendingSignUp => pendingSignUp.BusinessName).HasMaxLength(Business.NameMaxLength).IsRequired();
        builder.Property(pendingSignUp => pendingSignUp.CountryCode).HasMaxLength(Country.CodeLength).IsFixedLength().IsRequired();
        builder.Property(pendingSignUp => pendingSignUp.TimeZoneId).HasMaxLength(Business.TimeZoneIdMaxLength).IsRequired();
        builder.Property(pendingSignUp => pendingSignUp.CurrencyCode).HasMaxLength(Business.CurrencyCodeLength).IsFixedLength().IsRequired();
        builder.Property(pendingSignUp => pendingSignUp.TokenHash).HasMaxLength(PendingSignUp.TokenHashLength).IsFixedLength().IsRequired();
        builder.Ignore(pendingSignUp => pendingSignUp.Business);
        builder.HasIndex(pendingSignUp => pendingSignUp.TokenHash).IsUnique();
        builder.HasIndex(pendingSignUp => pendingSignUp.Email);
    }
}
