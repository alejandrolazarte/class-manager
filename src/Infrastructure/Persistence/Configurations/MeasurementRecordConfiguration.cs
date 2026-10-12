using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class MeasurementRecordConfiguration : IEntityTypeConfiguration<MeasurementRecord>
{
    public void Configure(EntityTypeBuilder<MeasurementRecord> builder)
    {
        builder.HasKey(record => record.Id);
        builder.Property(record => record.Id).ValueGeneratedNever();
        builder.Property(record => record.Value).HasPrecision(MeasurementValue.Precision, MeasurementValue.Decimals);
        builder.Property(record => record.Note).HasMaxLength(MeasurementRecord.NoteMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(record => record.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(record => record.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<MeasurementDefinition>().WithMany().HasForeignKey(record => record.MeasurementDefinitionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(record => new { record.TenantId, record.StudentId, record.MeasurementDefinitionId, record.MeasuredOn })
            .IsUnique()
            .HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
    }
}
