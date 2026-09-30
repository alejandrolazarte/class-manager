using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassFeedbackConfiguration : IEntityTypeConfiguration<ClassFeedback>
{
    public void Configure(EntityTypeBuilder<ClassFeedback> builder)
    {
        builder.HasKey(feedback => feedback.Id);
        builder.Property(feedback => feedback.Id).ValueGeneratedNever();
        builder.Property(feedback => feedback.Text).HasMaxLength(ClassFeedback.TextMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(feedback => feedback.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassSession>().WithMany().HasForeignKey(feedback => feedback.ClassSessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(feedback => feedback.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Instructor>().WithMany().HasForeignKey(feedback => feedback.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(feedback => new { feedback.TenantId, feedback.ClassSessionId, feedback.StudentId }).IsUnique();
        builder.HasIndex(feedback => new { feedback.TenantId, feedback.StudentId });
    }
}
