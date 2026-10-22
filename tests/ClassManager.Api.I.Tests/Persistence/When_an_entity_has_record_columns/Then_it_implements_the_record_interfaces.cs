using ClassManager.Records;

namespace ClassManager.Api.I.Tests.Persistence.When_an_entity_has_record_columns;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_implements_the_record_interfaces(ApiFixture fixture)
{
    private const string ReplacedDeletedOnColumn = "DeletedOn";

    private static readonly Dictionary<string, Type[]> InterfacesByColumn = new Dictionary<string, Type[]>
    {
        [nameof(ICreatedOn.CreatedOn)] = [typeof(ICreatedOn)],
        [nameof(IExpiredOn.ExpiredOn)] = [typeof(IExpiredOn)],
        [nameof(ISoftDeletable.IsDeleted)] = [typeof(ISoftDeletable)],
        [nameof(ISoftDeletable.DeletedAt)] = [typeof(ISoftDeletable)],
        [nameof(ISoftDeletable.DeletedByUserId)] = [typeof(ISoftDeletable)],
        [nameof(IAuditable.CreatedByUserId)] = [typeof(IAuditable)],
        [nameof(IAuditable.UpdatedAt)] = [typeof(IAuditable)],
        [nameof(IAuditable.UpdatedByUserId)] = [typeof(IAuditable)],
        [ReplacedDeletedOnColumn] = [],
    };

    [Fact]
    public void Then_it_implements_the_record_interfaces_Run()
    {
        using var context = fixture.CreateDbContext(0L);

        var violations = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties()
                .Where(property => InterfacesByColumn.ContainsKey(property.Name))
                .Where(property => !InterfacesByColumn[property.Name].Any(recordInterface => recordInterface.IsAssignableFrom(entityType.ClrType)))
                .Select(property => $"{entityType.ClrType.Name}.{property.Name}"))
            .ToList();

        violations.ShouldBeEmpty();
    }
}
