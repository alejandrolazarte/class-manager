using ClassManager.Backoffice.Core.Operators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Backoffice.Infrastructure.Persistence.Configurations;

internal sealed class BackofficeAccessRequestConfiguration : IEntityTypeConfiguration<BackofficeAccessRequest>
{
    public void Configure(EntityTypeBuilder<BackofficeAccessRequest> builder)
    {
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Provider).HasMaxLength(BackofficeLogin.ProviderMaxLength).IsRequired();
        builder.Property(request => request.ProviderKey).HasMaxLength(BackofficeLogin.ProviderKeyMaxLength).IsRequired();
        builder.Property(request => request.Email).HasMaxLength(BackofficeLogin.EmailMaxLength).IsRequired();
        builder.Property(request => request.Code).HasMaxLength(BackofficeAccessRequest.CodeMaxLength).IsRequired();
        builder.HasIndex(request => new { request.Provider, request.ProviderKey }).IsUnique();
    }
}
