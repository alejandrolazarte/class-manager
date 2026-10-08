namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_account_tries_to_authorize_a_sibling;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var clientApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);
        var olderChild = await business.HttpClient.AddStudentAsync(family.Id, "Lucía Pérez");
        var olderChildEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostChildAppInvitationAsync(family.Id, olderChild.Id, olderChildEmail, StudentAppRequests.ChildBirthDate)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(olderChildEmail));
        var olderChildTokens = (await accepted.Content.ReadFromJsonAsync<Core.UseCases.Authentication.TokenResponse>(ApiRequests.JsonOptions))!;
        using var olderChildApp = fixture.CreateClientWithToken(olderChildTokens.AccessToken);
        (await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, StudentAppRequests.UniqueStudentEmail(), StudentAppRequests.ElevenYearsOld)).EnsureSuccessStatusCode();
        var invitationId = (await clientApp.GetStudentAppHomeAsync())!.PendingGuardianConsents.Single().InvitationId;

        using var response = await olderChildApp.PostGuardianConsentInAppAsync(invitationId);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
