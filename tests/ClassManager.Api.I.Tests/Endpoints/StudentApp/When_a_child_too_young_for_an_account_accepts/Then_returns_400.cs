using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_too_young_for_an_account_accepts;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var child = family.Students.Single();
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        var today = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime);
        using (var invitation = await business.HttpClient.PostChildAppInvitationAsync(family.Id, child.Id, childEmail, today.AddYears(-15)))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        (await business.HttpClient.PutStudentAsync(child.Id, new UpdateStudentRequest(child.FullName, today.AddYears(-10), null, childEmail)))
            .EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.TooYoungForOwnAccount);
    }
}
