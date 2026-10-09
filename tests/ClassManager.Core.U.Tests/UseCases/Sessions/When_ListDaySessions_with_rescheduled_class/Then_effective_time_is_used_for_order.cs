using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListDaySessions_with_rescheduled_class;

public sealed class Then_effective_time_is_used_for_order
{
    [Fact]
    public async Task Then_effective_time_is_used_for_order_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var laterClassGroup = ClassGroup.Create(
            "Aquagym", builder.Instructor.Id, ClassSchedule.Create([TestData.Today.DayOfWeek], "18:30", 45).Value!, 10, null).Value!;
        var movedSession = ClassSession.Create(builder.ClassGroup.Id, TestData.Today, TestData.Now);
        movedSession.Reschedule(new TimeOnly(19, 30), builder.ClassGroup.DurationMinutes, builder.ClassGroup.StartTime, TestData.Now);
        builder.ClassGroups.Setup(repository => repository.ListActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([builder.ClassGroup, laterClassGroup]);
        builder.Instructors.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([builder.Instructor]);
        builder.Sessions.Setup(repository => repository.ListByDateAsync(TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync([movedSession]);
        builder.Enrollments.Setup(repository => repository.CountActiveOnByClassGroupAsync(TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(new Dictionary<Guid, int>());
        builder.Attendances
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, AttendanceCount>());

        var response = await builder.BuildListDay().ExecuteAsync(new ListDaySessionsQuery(null), CancellationToken.None);

        response.Value!.Select(session => session.ClassGroupName).ShouldBe(["Aquagym", "Natación inicial"]);
        response.Value![1].StartTime.ShouldBe("19:30");
        response.Value![1].OriginalStartTime.ShouldBe("18:00");
    }
}
