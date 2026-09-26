using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_restoring_session_schedule;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_day_shows_the_usual_time(ApiFixture fixture)
{
    [Fact]
    public async Task Then_day_shows_the_usual_time_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var schedulePath = $"{SessionRequests.SessionPath(classGroup.Id, EnrollmentRequests.Today)}{ApiRoutes.Schedule}";
        (await business.HttpClient.PutAsJsonAsync(schedulePath, new RescheduleSessionRequest("19:00"), ApiRequests.JsonOptions))
            .EnsureSuccessStatusCode();

        using var response = await business.HttpClient.DeleteAsync(new Uri(schedulePath, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var day = await business.HttpClient.GetDayAsync(EnrollmentRequests.Today);
        day!.Single().StartTime.ShouldBe("18:00");
        day![0].OriginalStartTime.ShouldBeNull();
    }
}
