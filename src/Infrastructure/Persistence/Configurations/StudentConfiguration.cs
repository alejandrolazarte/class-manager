using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(student => student.Id);
        builder.Property(student => student.Id).ValueGeneratedNever();
        builder.Property(student => student.FullName).HasMaxLength(Student.FullNameMaxLength).IsRequired();
        builder.Property(student => student.Notes).HasMaxLength(Student.NotesMaxLength);
        builder.Property(student => student.Email).HasMaxLength(Student.EmailMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(student => student.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(student => student.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(student => new { student.TenantId, student.ClientId, student.FullName }).IsUnique();
        builder.HasIndex(student => new { student.TenantId, student.FullName });
    }
}
