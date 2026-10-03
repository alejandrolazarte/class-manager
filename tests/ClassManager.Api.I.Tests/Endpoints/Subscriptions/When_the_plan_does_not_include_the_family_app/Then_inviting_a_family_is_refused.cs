using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_family_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_inviting_a_family_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_inviting_a_family_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);
        var client = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostFamilyInvitationAsync(client.Id, "familia@example.com");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.FamilyApp);
    }
}
