using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class PrivateLessonConfiguration : IEntityTypeConfiguration<PrivateLesson>
{
    public void Configure(EntityTypeBuilder<PrivateLesson> builder)
    {
        builder.HasKey(lesson => lesson.Id);
        builder.Property(lesson => lesson.Id).ValueGeneratedNever();
        builder.Property(lesson => lesson.Location).HasMaxLength(PrivateLesson.LocationMaxLength);
        builder.Property(lesson => lesson.Notes).HasMaxLength(PrivateLesson.NotesMaxLength);
        builder.Property(lesson => lesson.CancellationReason).HasMaxLength(PrivateLesson.CancellationReasonMaxLength);
        builder.Property(lesson => lesson.TrialPrice).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Ignore(lesson => lesson.EndTime);
        builder.Ignore(lesson => lesson.HasAttendance);

        builder.HasMany(lesson => lesson.Students)
            .WithOne()
            .HasForeignKey(lessonStudent => lessonStudent.PrivateLessonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(lesson => lesson.Students).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Business>().WithMany().HasForeignKey(lesson => lesson.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Instructor>().WithMany().HasForeignKey(lesson => lesson.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(lesson => new { lesson.TenantId, lesson.Date });
        builder.HasIndex(lesson => new { lesson.TenantId, lesson.InstructorId, lesson.Date });
    }
}
