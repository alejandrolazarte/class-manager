using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassGroupConfiguration : IEntityTypeConfiguration<ClassGroup>
{
    public void Configure(EntityTypeBuilder<ClassGroup> builder)
    {
        builder.HasKey(classGroup => classGroup.Id);
        builder.Property(classGroup => classGroup.Id).ValueGeneratedNever();
        builder.Property(classGroup => classGroup.Name).HasMaxLength(ClassGroup.NameMaxLength).IsRequired();
        builder.Property(classGroup => classGroup.Weekdays).HasConversion<int>();
        builder.Property(classGroup => classGroup.Location).HasMaxLength(ClassGroup.LocationMaxLength);
        builder.Property(classGroup => classGroup.MaterialUrl).HasMaxLength(ClassGroup.MaterialUrlMaxLength);
        builder.Ignore(classGroup => classGroup.Schedule);
        builder.HasOne(classGroup => classGroup.MaterialDocument)
            .WithMany()
            .HasForeignKey(classGroup => classGroup.MaterialDocumentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(classGroup => classGroup.MaterialDocumentId).IsUnique().HasFilter("[MaterialDocumentId] IS NOT NULL");
        builder.Navigation(classGroup => classGroup.MaterialDocument).AutoInclude();
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_ClassGroups_OneMaterial", "[MaterialUrl] IS NULL OR [MaterialDocumentId] IS NULL"));

        builder.HasOne<Business>().WithMany().HasForeignKey(classGroup => classGroup.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Instructor>().WithMany().HasForeignKey(classGroup => classGroup.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(classGroup => new { classGroup.TenantId, classGroup.IsActive, classGroup.StartTime });
    }
}
