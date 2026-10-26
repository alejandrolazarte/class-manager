using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    private const int StatusMaxLength = 16;

    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasKey(attendance => attendance.Id);
        builder.Property(attendance => attendance.Status).HasConversion<string>().HasMaxLength(StatusMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(attendance => attendance.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(attendance => attendance.Session).WithMany().HasForeignKey(attendance => attendance.ClassSessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(attendance => attendance.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(attendance => new { attendance.TenantId, attendance.ClassSessionId, attendance.StudentId })
            .IsUnique()
            .IncludeProperties(attendance => attendance.Status);
        builder.HasIndex(attendance => new { attendance.TenantId, attendance.StudentId })
            .IncludeProperties(attendance => new { attendance.ClassSessionId, attendance.Status });
    }
}
