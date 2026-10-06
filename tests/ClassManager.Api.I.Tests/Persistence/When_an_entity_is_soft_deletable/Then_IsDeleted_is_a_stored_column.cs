using ClassManager.Records;

namespace ClassManager.Api.I.Tests.Persistence.When_an_entity_is_soft_deletable;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_IsDeleted_is_a_stored_column(ApiFixture fixture)
{
    [Fact]
    public void Then_IsDeleted_is_a_stored_column_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var withoutStoredIsDeleted = context.Model.GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            .Where(entityType => entityType.FindProperty(nameof(ISoftDeletable.IsDeleted)) is null)
            .Select(entityType => entityType.ClrType.Name)
            .ToList();

        withoutStoredIsDeleted.ShouldBeEmpty();
    }
}
