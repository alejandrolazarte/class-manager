using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_custom_roles;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_creating_a_role_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_creating_a_role_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);

        using var response = await business.HttpClient.PostRoleAsync(RoleRequests.CustomRoleName, [Permissions.Students.ViewAll]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.NotInPlan);
    }
}
