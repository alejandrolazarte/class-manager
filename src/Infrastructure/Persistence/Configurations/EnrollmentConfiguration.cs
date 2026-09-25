using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    private const string OpenEnrollmentFilter = "[EndDate] IS NULL";

    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(enrollment => enrollment.Id);
        builder.Property(enrollment => enrollment.Id).ValueGeneratedNever();

        builder.HasOne<Business>().WithMany().HasForeignKey(enrollment => enrollment.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(enrollment => enrollment.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassGroup>().WithMany().HasForeignKey(enrollment => enrollment.ClassGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(enrollment => new { enrollment.TenantId, enrollment.StudentId, enrollment.ClassGroupId })
            .IsUnique()
            .HasFilter(OpenEnrollmentFilter);
        builder.HasIndex(enrollment => new { enrollment.TenantId, enrollment.ClassGroupId, enrollment.EndDate });
    }
}
