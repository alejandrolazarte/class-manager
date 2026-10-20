using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_a_reader_changes_the_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_forbidden(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_forbidden_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture, BusinessRole.PlatformReader);

        using var response = await changes.ChangePlanAsync("lite");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await changes.GetBrandAsync()).Subscription!.PlanCode.ShouldBe("pro");
    }
}
