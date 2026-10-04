using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_student_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_inviting_a_student_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_inviting_a_student_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);
        var client = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostStudentAppInvitationAsync(client.Id, "alumno@example.com");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.StudentApp);
    }
}
