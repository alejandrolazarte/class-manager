using ClassManager.Core.Domain.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_is_invited_with_the_client_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);

        using var response = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, clientEmail, StudentAppRequests.ChildBirthDate);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(StudentErrorCodes.EmailOfAnotherPerson);
    }
}
