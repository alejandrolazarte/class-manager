namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_client_uses_the_app_and_a_child_needs_authorization;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_home_lists_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_home_lists_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var clientApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);
        (await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, StudentAppRequests.UniqueStudentEmail(), StudentAppRequests.ElevenYearsOld)).EnsureSuccessStatusCode();

        var home = await clientApp.GetStudentAppHomeAsync();

        home!.PendingGuardianConsents.Single().StudentFullName.ShouldBe(StudentAppRequests.ChildFullName);
    }
}
