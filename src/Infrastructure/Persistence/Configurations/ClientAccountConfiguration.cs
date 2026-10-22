using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClientAccountConfiguration : IEntityTypeConfiguration<ClientAccount>
{
    public void Configure(EntityTypeBuilder<ClientAccount> builder)
    {
        builder.HasKey(account => account.Id);

        builder.HasOne<Business>().WithMany().HasForeignKey(account => account.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(account => account.User).WithMany().HasForeignKey(account => account.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(account => account.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(account => account.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(account => new { account.TenantId, account.UserId }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
        builder.HasIndex(account => new { account.TenantId, account.ClientId });
    }
}
