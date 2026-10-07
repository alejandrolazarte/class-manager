namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_uses_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_can_still_be_invited(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_can_still_be_invited_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        using (var childInvitation = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ChildBirthDate))
        {
            childInvitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using (var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail)))
        {
            accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        using var response = await business.HttpClient.PostStudentAppInvitationAsync(family.Id, clientEmail);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
