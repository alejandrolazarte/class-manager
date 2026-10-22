using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClassManager.Api.I.Tests.Persistence.When_an_entity_has_an_id;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_database_generates_it(ApiFixture fixture)
{
    private const string IdPropertyName = "Id";

    [Fact]
    public void Then_the_database_generates_it_Run()
    {
        using var context = fixture.CreateDbContext(0L);

        var notGenerated = context.Model.GetEntityTypes()
            .Where(entityType => !entityType.IsOwned())
            .Select(entityType => entityType.FindPrimaryKey())
            .OfType<IKey>()
            .Where(key => key.Properties is [{ Name: IdPropertyName }])
            .Select(key => key.Properties[0])
            .Where(id => id.ClrType != typeof(long) || id.ValueGenerated != ValueGenerated.OnAdd || id.GetValueGenerationStrategy() != SqlServerValueGenerationStrategy.IdentityColumn)
            .Select(id => id.DeclaringType.Name)
            .ToList();

        notGenerated.ShouldBeEmpty();
    }
}
