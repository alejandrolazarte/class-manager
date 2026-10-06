using ClassManager.Infrastructure.Persistence;
using ClassManager.Records;
using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Api.I.Tests.Persistence.When_an_entity_is_soft_deletable;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_keeps_the_tenant_and_soft_delete_filters(ApiFixture fixture)
{
    [Fact]
    public void Then_it_keeps_the_tenant_and_soft_delete_filters_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var softDeletableEntityTypes = context.Model.GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            .ToList();
        var withoutBothFilters = softDeletableEntityTypes
            .Where(entityType =>
            {
                var filterKeys = entityType.GetDeclaredQueryFilters().Select(filter => filter.Key).ToList();
                return !filterKeys.Contains(SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter)
                    || !filterKeys.Contains(TenantModelBuilderExtensions.TenantQueryFilter);
            })
            .Select(entityType => entityType.ClrType.Name)
            .ToList();

        softDeletableEntityTypes.ShouldNotBeEmpty();
        withoutBothFilters.ShouldBeEmpty();
    }
}
