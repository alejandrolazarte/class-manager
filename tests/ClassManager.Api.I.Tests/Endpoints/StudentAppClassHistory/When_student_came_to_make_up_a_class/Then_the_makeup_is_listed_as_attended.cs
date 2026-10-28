using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_came_to_make_up_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_makeup_is_listed_as_attended(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_makeup_is_listed_as_attended_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var studentId = scenario.Instructors.InstructorStudentId;
        var otherClassGroupId = scenario.Instructors.OtherClassGroup.Id;
        var lastWeek = InstructorScenario.ClassDate.AddDays(-7);
        await using (var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id))
        {
            var session = ClassSession.Create(otherClassGroupId, lastWeek, BusinessApiFactory.Now.AddDays(-10));
            context.ClassSessions.Add(session);
            await context.SaveChangesAsync();
            context.MakeupBookings.Add(MakeupBooking.Create(session, studentId, BusinessApiFactory.Now.AddDays(-10)));
            await context.SaveChangesAsync();
        }

        (await scenario.Instructors.Business.HttpClient.PutAttendanceAsync(otherClassGroupId, lastWeek, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var history = await scenario.Student.GetClassHistoryAsync(studentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Single().ShouldSatisfyAllConditions(
            pastClass => pastClass.IsMakeup.ShouldBeTrue(),
            pastClass => pastClass.Status.ShouldBe(PastClassStatus.Attended));
    }
}
