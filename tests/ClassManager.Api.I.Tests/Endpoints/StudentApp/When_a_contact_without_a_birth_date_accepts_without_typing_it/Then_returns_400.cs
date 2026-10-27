namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_without_a_birth_date_accepts_without_typing_it;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(family.Id, contactEmail)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(
            fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail), birthDate: null);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
