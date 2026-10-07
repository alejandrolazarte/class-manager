using ClassManager.Core.Domain.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_without_birth_date_is_invited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());

        using var response = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, StudentAppRequests.UniqueStudentEmail(), null);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(StudentAppErrorCodes.BirthDateRequired);
    }
}
