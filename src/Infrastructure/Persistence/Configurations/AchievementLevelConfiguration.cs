using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class AchievementLevelConfiguration : IEntityTypeConfiguration<AchievementLevel>
{
    public void Configure(EntityTypeBuilder<AchievementLevel> builder)
    {
        builder.HasKey(level => level.Id);
        builder.Property(level => level.Id).ValueGeneratedNever();
        builder.Property(level => level.Name).HasMaxLength(LevelLadder.NameMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(level => level.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(level => new { level.TenantId, level.Position });
    }
}
