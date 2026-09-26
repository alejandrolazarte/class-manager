using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_rescheduling_session_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{SessionRequests.SessionPath(otherClassGroup.Id, EnrollmentRequests.Today)}{ApiRoutes.Schedule}",
            new RescheduleSessionRequest("19:00"),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
