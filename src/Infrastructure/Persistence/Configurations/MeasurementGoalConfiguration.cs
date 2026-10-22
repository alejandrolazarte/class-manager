using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class MeasurementGoalConfiguration : IEntityTypeConfiguration<MeasurementGoal>
{
    public void Configure(EntityTypeBuilder<MeasurementGoal> builder)
    {
        builder.HasKey(goal => goal.Id);
        builder.Property(goal => goal.Value).HasPrecision(MeasurementValue.Precision, MeasurementValue.Decimals);

        builder.HasOne<Business>().WithMany().HasForeignKey(goal => goal.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(goal => goal.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<MeasurementDefinition>().WithMany().HasForeignKey(goal => goal.MeasurementDefinitionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(goal => new { goal.TenantId, goal.StudentId, goal.MeasurementDefinitionId }).IsUnique();
    }
}
