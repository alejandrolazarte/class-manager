using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AbsenceNoticeConfiguration : IEntityTypeConfiguration<AbsenceNotice>
{
    public void Configure(EntityTypeBuilder<AbsenceNotice> builder)
    {
        builder.HasKey(notice => notice.Id);
        builder.Property(notice => notice.Id).ValueGeneratedNever();

        builder.HasOne<Business>().WithMany().HasForeignKey(notice => notice.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassSession>().WithMany().HasForeignKey(notice => notice.ClassSessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(notice => notice.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(notice => new { notice.TenantId, notice.ClassSessionId, notice.StudentId }).IsUnique();
        builder.HasIndex(notice => new { notice.TenantId, notice.StudentId });
    }
}
