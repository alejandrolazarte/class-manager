using System.Net.Http.Json;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Api.I.Tests.Operators.When_an_operator_enables_an_access_request;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_person_signs_in_with_the_chosen_role(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_person_signs_in_with_the_chosen_role_Run()
    {
        var person = await fixture.SeedPlatformTeamMemberAsync();
        using var personClient = fixture.ApiFactory.CreateHttpsClient();
        using (var denied = await personClient.SignInWithGoogleAsync(person.Subject, person.Email))
        {
            BackofficeRequests.RefreshTokenCookieOf(denied).ShouldBeNull();
        }

        using var operatorClient = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await operatorClient.OpenSessionAsync(await fixture.SeedOperatorAsync());
        using var list = await operatorClient.GetAsync(BackofficeRequests.OperatorsRoute, accessToken);
        var accessRequest = (await list.Content.ReadFromJsonAsync<OperatorListResponse>())!.AccessRequests
            .Single(request => request.Email == person.Email);
        using var enable = await operatorClient.SendAsync(
            HttpMethod.Post,
            BackofficeRequests.OperatorsRoute,
            accessToken,
            new { accessRequestId = accessRequest.Id, memberId = person.MemberId, role = nameof(BusinessRole.PlatformReader) });
        enable.StatusCode.ShouldBe(HttpStatusCode.OK);

        var personAccessToken = await personClient.OpenSessionAsync(person);

        using var currentOperator = await personClient.GetCurrentOperatorAsync(personAccessToken);
        (await currentOperator.Content.ReadAsStringAsync()).ShouldContain(nameof(BusinessRole.PlatformReader));
    }
}
