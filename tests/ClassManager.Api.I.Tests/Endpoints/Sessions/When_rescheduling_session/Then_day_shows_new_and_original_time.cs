using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_rescheduling_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_day_shows_new_and_original_time(ApiFixture fixture)
{
    [Fact]
    public async Task Then_day_shows_new_and_original_time_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var nextThursday = EnrollmentRequests.Today.AddDays(7);

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{SessionRequests.SessionPath(classGroup.Id, nextThursday)}{ApiRoutes.Schedule}",
            new RescheduleSessionRequest("19:00"),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var day = await business.HttpClient.GetDayAsync(nextThursday);
        day!.Single().StartTime.ShouldBe("19:00");
        day![0].EndTime.ShouldBe("19:45");
        day![0].OriginalStartTime.ShouldBe("18:00");
    }
}
