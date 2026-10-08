namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_student_invitation_is_declined;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_can_no_longer_be_accepted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_can_no_longer_be_accepted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(email);
        using (var invitation = await business.HttpClient.PostStudentAppInvitationAsync(family.Id, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var token = fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email);
        using var anonymous = fixture.ApiFactory.CreateClient();
        using (var declined = await anonymous.PostDeclineStudentAppInvitationAsync(token))
        {
            declined.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
