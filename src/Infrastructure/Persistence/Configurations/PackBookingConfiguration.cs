using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class PackBookingConfiguration : IEntityTypeConfiguration<PackBooking>
{
    public void Configure(EntityTypeBuilder<PackBooking> builder)
    {
        builder.HasKey(booking => booking.Id);
        builder.Property(booking => booking.Id).ValueGeneratedNever();

        builder.HasOne<Business>().WithMany().HasForeignKey(booking => booking.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassSession>().WithMany().HasForeignKey(booking => booking.ClassSessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(booking => booking.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(booking => new { booking.TenantId, booking.ClassSessionId, booking.StudentId }).IsUnique();
        builder.HasIndex(booking => new { booking.TenantId, booking.StudentId });
    }
}
