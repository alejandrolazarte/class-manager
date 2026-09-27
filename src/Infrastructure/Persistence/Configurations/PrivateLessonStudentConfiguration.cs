using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class PrivateLessonStudentConfiguration : IEntityTypeConfiguration<PrivateLessonStudent>
{
    private const int StatusMaxLength = 16;

    public void Configure(EntityTypeBuilder<PrivateLessonStudent> builder)
    {
        builder.HasKey(lessonStudent => lessonStudent.Id);
        builder.Property(lessonStudent => lessonStudent.Id).ValueGeneratedNever();
        builder.Property(lessonStudent => lessonStudent.Status).HasConversion<string>().HasMaxLength(StatusMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(lessonStudent => lessonStudent.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(lessonStudent => lessonStudent.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(lessonStudent => new { lessonStudent.TenantId, lessonStudent.PrivateLessonId, lessonStudent.StudentId }).IsUnique();
        builder.HasIndex(lessonStudent => new { lessonStudent.TenantId, lessonStudent.StudentId });
    }
}
