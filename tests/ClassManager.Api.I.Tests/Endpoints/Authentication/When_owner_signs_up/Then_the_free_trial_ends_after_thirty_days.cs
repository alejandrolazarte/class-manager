using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_owner_signs_up;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_free_trial_ends_after_thirty_days(ApiFixture fixture)
{
    private const int TrialDays = 30;

    [Fact]
    public async Task Then_the_free_trial_ends_after_thirty_days_Run()
    {
        var businessName = $"Trial brand {Guid.NewGuid():N}";
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        await anonymousClient.SignUpAsync(AuthenticationRequests.SignUpCommand(businessName: businessName));

        await using var context = fixture.CreateDbContext(Guid.Empty);
        var organizationId = await context.Organizations.Where(organization => organization.Name == businessName).Select(organization => organization.Id).SingleAsync();
        var subscription = await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == organizationId);
        subscription.EndsOn.ShouldBe(DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(TrialDays - 1));
    }
}
