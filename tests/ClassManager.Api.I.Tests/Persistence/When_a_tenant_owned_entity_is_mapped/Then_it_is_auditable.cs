using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Api.I.Tests.Persistence.When_a_tenant_owned_entity_is_mapped;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_auditable(ApiFixture fixture)
{
    [Fact]
    public void Then_it_is_auditable_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var notAuditable = context.Model.GetEntityTypes()
            .Select(entityType => entityType.ClrType)
            .Where(entityType => typeof(ITenantOwned).IsAssignableFrom(entityType) && !typeof(IAuditable).IsAssignableFrom(entityType))
            .Select(entityType => entityType.Name)
            .ToList();

        notAuditable.ShouldBeEmpty();
    }
}
