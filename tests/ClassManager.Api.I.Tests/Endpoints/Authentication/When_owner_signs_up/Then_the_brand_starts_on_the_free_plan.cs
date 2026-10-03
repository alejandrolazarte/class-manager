using ClassManager.Core.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_owner_signs_up;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_starts_on_the_free_plan(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_starts_on_the_free_plan_Run()
    {
        var businessName = $"Free brand {Guid.NewGuid():N}";
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        await anonymousClient.SignUpAsync(AuthenticationRequests.SignUpCommand(businessName: businessName));

        await using var context = fixture.CreateDbContext(Guid.Empty);
        var organizationId = await context.Organizations.Where(organization => organization.Name == businessName).Select(organization => organization.Id).SingleAsync();
        var subscription = await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == organizationId);
        subscription.PlanCode.ShouldBe(PlanCodes.Free);
        subscription.Price.ShouldBe(0m);
    }
}
