using ClassManager.Infrastructure.BackgroundTasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class BackgroundTaskConfiguration : IEntityTypeConfiguration<BackgroundTask>
{
    private const string PendingFilter = "[FailedOn] IS NULL";

    public void Configure(EntityTypeBuilder<BackgroundTask> builder)
    {
        builder.HasKey(task => task.Id);
        builder.Property(task => task.Id).ValueGeneratedNever();
        builder.Property(task => task.Type).HasMaxLength(BackgroundTask.TypeMaxLength).IsRequired();
        builder.Property(task => task.Payload).IsRequired();
        builder.Property(task => task.LastError).HasMaxLength(BackgroundTask.LastErrorMaxLength);

        builder.HasIndex(task => task.NextAttemptOn).HasFilter(PendingFilter);
    }
}
