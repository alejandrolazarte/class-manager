using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();
        builder.Property(session => session.CancellationReason).HasMaxLength(ClassSession.CancellationReasonMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(session => session.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassGroup>().WithMany().HasForeignKey(session => session.ClassGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(session => new { session.TenantId, session.ClassGroupId, session.Date }).IsUnique();
        builder.HasIndex(session => new { session.TenantId, session.Date });
    }
}
