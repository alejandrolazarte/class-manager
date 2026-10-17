using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.HasKey(instructor => instructor.Id);
        builder.Property(instructor => instructor.Id).ValueGeneratedNever();

        builder.HasOne<Business>().WithMany().HasForeignKey(instructor => instructor.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(instructor => instructor.User).WithMany().HasForeignKey(instructor => instructor.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
