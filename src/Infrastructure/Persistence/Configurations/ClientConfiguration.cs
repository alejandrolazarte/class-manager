using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public const string RetiredMonthlyFeeColumn = "MonthlyFee";

    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(client => client.Id);
        builder.Property(client => client.Notes).HasMaxLength(Client.NotesMaxLength);
        builder.Property<decimal?>(RetiredMonthlyFeeColumn).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);

        builder.HasOne<Business>().WithMany().HasForeignKey(client => client.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(client => client.User).WithMany().HasForeignKey(client => client.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(client => new { client.TenantId, client.RegisteredByUserId });
    }
}
