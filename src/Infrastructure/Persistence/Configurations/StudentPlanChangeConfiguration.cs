using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class StudentPlanChangeConfiguration : IEntityTypeConfiguration<StudentPlanChange>
{
    public void Configure(EntityTypeBuilder<StudentPlanChange> builder)
    {
        builder.HasKey(change => change.Id);
        builder.Property(change => change.CustomPrice).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Ignore(change => change.HasPlan);

        builder.HasOne<Business>().WithMany().HasForeignKey(change => change.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(change => change.Student).WithMany().HasForeignKey(change => change.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassPack>().WithMany().HasForeignKey(change => change.ClassPackId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(change => new { change.TenantId, change.StudentId, change.EffectiveFrom }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
    }
}
