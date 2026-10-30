using ClassManager.Core.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_the_feature_catalog_is_seeded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_branches_document_storage_and_self_service_billing_are_add_ons(ApiFixture fixture)
{
    private const decimal ExtraBranchPrice = 20m;

    [Fact]
    public async Task Then_only_branches_document_storage_and_self_service_billing_are_add_ons_Run()
    {
        await using var context = fixture.CreateDbContext(0L);

        var addOns = await context.Features.Where(feature => feature.IsAddOn).ToListAsync();

        addOns.Select(feature => feature.Code).Order(StringComparer.Ordinal)
            .ShouldBe(new[] { Features.Branches, Features.DocumentStorage, Features.SelfServiceBilling }.Order(StringComparer.Ordinal));
        addOns.Single(feature => feature.Code == Features.Branches).AddOnListPrice.ShouldBe(ExtraBranchPrice);
        addOns.Single(feature => feature.Code == Features.SelfServiceBilling).AddOnListPrice.ShouldBe(0m);
    }
}
