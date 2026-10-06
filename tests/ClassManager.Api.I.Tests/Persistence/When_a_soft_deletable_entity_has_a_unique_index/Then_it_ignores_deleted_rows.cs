using ClassManager.Records;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_soft_deletable_entity_has_a_unique_index;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_ignores_deleted_rows(ApiFixture fixture)
{
    private const string NotDeletedFilter = "[IsDeleted] = 0";

    [Fact]
    public void Then_it_ignores_deleted_rows_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var indexesCountingDeletedRows = context.Model.GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            .SelectMany(entityType => entityType.GetIndexes()
                .Where(index => index.IsUnique && index.GetFilter()?.Contains(NotDeletedFilter, StringComparison.Ordinal) != true)
                .Select(index => $"{entityType.ClrType.Name}({string.Join(", ", index.Properties.Select(property => property.Name))})"))
            .ToList();

        indexesCountingDeletedRows.ShouldBeEmpty();
    }
}
