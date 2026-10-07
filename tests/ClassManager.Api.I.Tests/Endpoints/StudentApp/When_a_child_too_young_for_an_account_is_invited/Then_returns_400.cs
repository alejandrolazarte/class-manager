using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_too_young_for_an_account_is_invited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var tenYearsAgo = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddYears(-10);

        using var response = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, StudentAppRequests.UniqueStudentEmail(), tenYearsAgo);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.TooYoungForOwnAccount);
    }
}
