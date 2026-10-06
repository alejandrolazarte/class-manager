using ClassManager.Records;

namespace ClassManager.Api.I.Tests.Persistence.When_an_entity_has_record_columns;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_implements_the_record_interfaces(ApiFixture fixture)
{
    private static readonly Dictionary<string, Type[]> InterfacesByColumn = new Dictionary<string, Type[]>
    {
        [nameof(ICreatedOn.CreatedOn)] = [typeof(ICreatedOn)],
        [nameof(IDeletedOn.DeletedOn)] = [typeof(IDeletedOn), typeof(ISoftDeletable)],
        [nameof(IExpiredOn.ExpiredOn)] = [typeof(IExpiredOn)],
    };

    [Fact]
    public void Then_it_implements_the_record_interfaces_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var violations = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties()
                .Where(property => InterfacesByColumn.ContainsKey(property.Name))
                .Where(property => !InterfacesByColumn[property.Name].Any(recordInterface => recordInterface.IsAssignableFrom(entityType.ClrType)))
                .Select(property => $"{entityType.ClrType.Name}.{property.Name}"))
            .ToList();

        violations.ShouldBeEmpty();
    }
}
