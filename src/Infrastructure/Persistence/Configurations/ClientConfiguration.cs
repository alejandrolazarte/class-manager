using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public const string RetiredMonthlyFeeColumn = "MonthlyFee";

    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(client => client.Id);
        builder.Property(client => client.Id).ValueGeneratedNever();
        builder.Property(client => client.FullName).HasMaxLength(Client.FullNameMaxLength).IsRequired();
        builder.Property(client => client.PhoneNumber)
            .HasConversion(phoneNumber => phoneNumber.Value, value => PhoneNumber.FromNormalized(value))
            .HasMaxLength(PhoneNumber.MaxLength)
            .IsRequired();
        builder.Property(client => client.Email).HasMaxLength(Client.EmailMaxLength);
        builder.Property(client => client.Notes).HasMaxLength(Client.NotesMaxLength);
        builder.Property<decimal?>(RetiredMonthlyFeeColumn).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);

        builder.HasOne<Business>().WithMany().HasForeignKey(client => client.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(client => new { client.TenantId, client.PhoneNumber }).IsUnique();
        builder.HasIndex(client => new { client.TenantId, client.FullName });
    }
}
