using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(student => student.Id);
        builder.Property(student => student.Notes).HasMaxLength(Student.NotesMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(student => student.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(student => student.User).WithMany().HasForeignKey(student => student.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(student => student.Client).WithMany().HasForeignKey(student => student.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(student => new { student.TenantId, student.ClientId })
            .IncludeProperties(student => new { student.UserId, student.Notes });
    }
}
