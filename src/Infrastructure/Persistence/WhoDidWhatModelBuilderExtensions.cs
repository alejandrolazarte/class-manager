namespace ClassManager.Infrastructure.Persistence;

internal static class WhoDidWhatModelBuilderExtensions
{
    private const string WhoDidWhatColumnSuffix = "ByUserId";

    public static ModelBuilder ApplyWhoDidWhatForeignKeys(this ModelBuilder modelBuilder)
    {
        var whoDidWhatColumns = modelBuilder.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties()
                .Where(property => property.Name.EndsWith(WhoDidWhatColumnSuffix, StringComparison.Ordinal))
                .Select(property => (EntityType: entityType.ClrType, ColumnName: property.Name)))
            .ToList();

        foreach (var (entityType, columnName) in whoDidWhatColumns)
        {
            modelBuilder.Entity(entityType)
                .HasOne(typeof(User))
                .WithMany()
                .HasForeignKey(columnName)
                .OnDelete(DeleteBehavior.Restrict);
        }

        return modelBuilder;
    }
}
