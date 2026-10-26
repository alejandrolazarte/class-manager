using System.Globalization;
using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_signs_in_to_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_token_carries_their_brand(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_token_carries_their_brand_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var organizationId = AuthenticationRequests.AccessTokenClaim(scenario.Tokens.AccessToken, SessionClaimTypes.OrganizationId);

        organizationId.ShouldBe(scenario.Instructors.Business.Business.OrganizationId.ToString(CultureInfo.InvariantCulture));
    }
}
