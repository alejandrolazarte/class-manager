using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Infrastructure.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_owner_signs_up_with_a_configured_sign_up_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_starts_on_that_plan(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_starts_on_that_plan_Run()
    {
        var businessName = $"Pro brand {Guid.NewGuid():N}";
        await using var apiFactory = new BusinessApiFactory(
            fixture.ConnectionString,
            new FakeTimeProvider(BusinessApiFactory.Now),
            new Dictionary<string, string> { [SubscriptionSettings.SignUpPlan] = PlanCodes.Pro });
        using var anonymousClient = apiFactory.CreateClient();

        await anonymousClient.SignUpAsync(AuthenticationRequests.SignUpCommand(businessName: businessName));

        await using var context = fixture.CreateDbContext(Guid.Empty);
        var organizationId = await context.Organizations.Where(organization => organization.Name == businessName).Select(organization => organization.Id).SingleAsync();
        (await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == organizationId)).PlanCode.ShouldBe(PlanCodes.Pro);
    }
}
