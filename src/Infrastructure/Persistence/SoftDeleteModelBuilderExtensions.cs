using System.Linq.Expressions;
using ClassManager.Records;

namespace ClassManager.Infrastructure.Persistence;

internal static class SoftDeleteModelBuilderExtensions
{
    public const string SoftDeleteQueryFilter = "SoftDelete";

    public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        var softDeletableEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            .ToList();

        foreach (var entityType in softDeletableEntityTypes)
        {
            var entity = Expression.Parameter(entityType.ClrType);
            var deletedOn = Expression.Property(entity, nameof(ISoftDeletable.DeletedOn));
            var filter = Expression.Lambda(Expression.Equal(deletedOn, Expression.Constant(null, typeof(DateTimeOffset?))), entity);

            var entityBuilder = modelBuilder.Entity(entityType.ClrType);
            entityBuilder.Ignore(nameof(ISoftDeletable.IsDeleted));
            entityBuilder.HasQueryFilter(SoftDeleteQueryFilter, filter);
        }

        return modelBuilder;
    }
}
