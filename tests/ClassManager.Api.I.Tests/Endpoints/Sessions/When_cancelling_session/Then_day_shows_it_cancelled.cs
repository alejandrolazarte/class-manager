using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_cancelling_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_day_shows_it_cancelled(ApiFixture fixture)
{
    [Fact]
    public async Task Then_day_shows_it_cancelled_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var nextThursday = EnrollmentRequests.Today.AddDays(7);

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{SessionRequests.SessionPath(classGroup.Id, nextThursday)}{ApiRoutes.Cancellation}",
            new CancelSessionRequest("Feriado"),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var day = await business.HttpClient.GetDayAsync(nextThursday);
        day!.Single().IsCancelled.ShouldBeTrue();
        day![0].CancellationReason.ShouldBe("Feriado");
    }
}
