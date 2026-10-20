using ClassManager.Backoffice.Core.Operators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Backoffice.Infrastructure.Persistence.Configurations;

internal sealed class BackofficeLoginConfiguration : IEntityTypeConfiguration<BackofficeLogin>
{
    public void Configure(EntityTypeBuilder<BackofficeLogin> builder)
    {
        builder.HasKey(login => login.Id);
        builder.Property(login => login.Provider).HasMaxLength(BackofficeLogin.ProviderMaxLength).IsRequired();
        builder.Property(login => login.ProviderKey).HasMaxLength(BackofficeLogin.ProviderKeyMaxLength).IsRequired();
        builder.Property(login => login.Email).HasMaxLength(BackofficeLogin.EmailMaxLength).IsRequired();
        builder.HasIndex(login => new { login.Provider, login.ProviderKey }).IsUnique();
        builder.HasIndex(login => login.AccountId);
    }
}
