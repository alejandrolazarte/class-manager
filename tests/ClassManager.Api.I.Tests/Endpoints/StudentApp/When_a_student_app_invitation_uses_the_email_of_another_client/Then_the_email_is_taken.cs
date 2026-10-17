using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_student_app_invitation_uses_the_email_of_another_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_is_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_is_taken_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var businessId = scenario.Instructors.Business.Business.Id;
        var otherClientId = await fixture.ClientIdOfStudentAsync(businessId, scenario.Instructors.OtherStudentId);

        using var response = await scenario.Instructors.Business.HttpClient.PostStudentAppInvitationAsync(otherClientId, scenario.Email);

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(UserErrorCodes.EmailTaken);
    }
}
