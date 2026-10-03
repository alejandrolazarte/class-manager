using ClassManager.Records;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_an_entity_keeps_replaced_records;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_current_rows_have_a_unique_index(ApiFixture fixture)
{
    private const string CurrentRecordFilter = "[DeletedOn] IS NULL";

    [Fact]
    public void Then_current_rows_have_a_unique_index_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var replaceableEntityTypes = context.Model.GetEntityTypes()
            .Where(entityType => typeof(IDeletedOn).IsAssignableFrom(entityType.ClrType))
            .ToList();
        var withoutCurrentIndex = replaceableEntityTypes
            .Where(entityType => !entityType.GetIndexes().Any(index => index.IsUnique && index.GetFilter() == CurrentRecordFilter))
            .Select(entityType => entityType.ClrType.Name)
            .ToList();

        replaceableEntityTypes.ShouldNotBeEmpty();
        withoutCurrentIndex.ShouldBeEmpty();
    }
}
