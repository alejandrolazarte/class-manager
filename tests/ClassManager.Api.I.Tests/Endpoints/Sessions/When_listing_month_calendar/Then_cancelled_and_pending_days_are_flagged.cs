using ClassManager.Core.UseCases.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_listing_month_calendar;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_cancelled_and_pending_days_are_flagged(ApiFixture fixture)
{
    [Fact]
    public async Task Then_cancelled_and_pending_days_are_flagged_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var twoWeeksAgo = EnrollmentRequests.Today.AddDays(-14);
        var lastWeek = EnrollmentRequests.Today.AddDays(-7);
        await business.HttpClient.EnrollAsync(classGroup.Id, client.Students[0].Id, twoWeeksAgo);
        using var cancellation = await business.HttpClient.PutAsJsonAsync(
            $"{SessionRequests.SessionPath(classGroup.Id, lastWeek)}{ApiRoutes.Cancellation}",
            new CancelSessionRequest("Feriado"),
            ApiRequests.JsonOptions);
        cancellation.EnsureSuccessStatusCode();

        var calendar = await business.HttpClient.GetMonthCalendarAsync("2026-09");

        var days = calendar!.Days.ToDictionary(day => day.Date);
        days[twoWeeksAgo].PendingAttendanceCount.ShouldBe(1);
        days[lastWeek].CancelledCount.ShouldBe(1);
        days[lastWeek].PendingAttendanceCount.ShouldBe(0);
        days[EnrollmentRequests.Today].PendingAttendanceCount.ShouldBe(0);
    }
}
