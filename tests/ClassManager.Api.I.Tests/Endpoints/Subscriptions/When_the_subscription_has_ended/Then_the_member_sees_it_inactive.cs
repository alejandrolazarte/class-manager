using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_subscription_has_ended;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_member_sees_it_inactive(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_member_sees_it_inactive_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await fixture.EndSubscriptionAsync(business.Business.Id);

        var member = await business.HttpClient.GetCurrentMemberAsync();

        member.Subscription!.IsActive.ShouldBeFalse();
        member.Subscription.PlanCode.ShouldBe(PlanCodes.Free);
        member.Subscription.ExpiredOn.ShouldBe(DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(-1));
        member.Subscription.Features.ShouldBeEmpty();
    }
}
