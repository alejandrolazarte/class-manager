using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_new_student_accepts_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_records_the_accepted_legal_terms(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_records_the_accepted_legal_terms_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var email = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(family.Id, email)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email)))
            .EnsureSuccessStatusCode();

        (await fixture.LegalTermsAcceptancesOfAsync(email)).ShouldHaveSingleItem().Version.ShouldBe(LegalTerms.CurrentVersion);
    }
}
