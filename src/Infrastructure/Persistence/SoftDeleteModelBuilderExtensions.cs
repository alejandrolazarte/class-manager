using System.Linq.Expressions;
using ClassManager.Records;

namespace ClassManager.Infrastructure.Persistence;

internal static class SoftDeleteModelBuilderExtensions
{
    public const string SoftDeleteQueryFilter = "SoftDelete";
    public const string NotDeletedFilter = "[IsDeleted] = 0";

    private const string CheckConstraintPrefix = "CK_";
    private const string CheckConstraintSuffix = "_IsDeleted_DeletedAt";
    private const string IsDeletedMatchesDeletedAtSql =
        "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)";

    public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        var softDeletableEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            .ToList();

        foreach (var entityType in softDeletableEntityTypes)
        {
            var entity = Expression.Parameter(entityType.ClrType);
            var isDeleted = Expression.Property(entity, nameof(ISoftDeletable.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeleted), entity);
            var checkConstraintName = CheckConstraintPrefix + entityType.GetTableName() + CheckConstraintSuffix;

            var entityBuilder = modelBuilder.Entity(entityType.ClrType);
            entityBuilder.ToTable(table => table.HasCheckConstraint(checkConstraintName, IsDeletedMatchesDeletedAtSql));
            entityBuilder.HasQueryFilter(SoftDeleteQueryFilter, filter);
        }

        return modelBuilder;
    }
}
