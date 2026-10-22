using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class MeasurementDefinitionConfiguration : IEntityTypeConfiguration<MeasurementDefinition>
{
    private const int EnumMaxLength = 16;

    public void Configure(EntityTypeBuilder<MeasurementDefinition> builder)
    {
        builder.HasKey(definition => definition.Id);
        builder.Property(definition => definition.Name).HasMaxLength(MeasurementDefinition.NameMaxLength).IsRequired();
        builder.Property(definition => definition.Kind).HasConversion<string>().HasMaxLength(EnumMaxLength);
        builder.Property(definition => definition.Unit).HasMaxLength(MeasurementUnits.MaxLength).IsRequired();
        builder.Property(definition => definition.ImprovesWhen).HasConversion<string>().HasMaxLength(EnumMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(definition => definition.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(definition => new { definition.TenantId, definition.Name }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
    }
}
