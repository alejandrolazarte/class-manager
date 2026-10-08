using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_too_young_for_an_account_accepts;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        var today = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime);
        var tenYearsAgo = today.AddYears(-10);
        using (var invitation = await business.HttpClient.PostChildAppInvitationAsync(family.Id, family.Students.Single().Id, childEmail, today.AddYears(-15)))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptStudentAppInvitation,
            new AcceptStudentAppInvitationCommand(
                fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail),
                null,
                StudentAppRequests.StudentAppPassword,
                tenYearsAgo),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.TooYoungForOwnAccount);
    }
}
