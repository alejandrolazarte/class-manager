using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListDaySessions;

public sealed class Then_only_classes_meeting_that_weekday_are_returned
{
    [Fact]
    public async Task Then_only_classes_meeting_that_weekday_are_returned_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var otherDayClassGroup = ClassGroup.Create(
            "Aquagym", builder.Instructor.Id, ClassSchedule.Create([TestData.Today.AddDays(1).DayOfWeek], "10:00", 60).Value!, 10, null).Value!;
        builder.ClassGroups.Setup(repository => repository.ListActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([builder.ClassGroup, otherDayClassGroup]);
        builder.Instructors.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([builder.Instructor]);
        builder.Sessions.Setup(repository => repository.ListByDateAsync(TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        builder.Enrollments.Setup(repository => repository.CountActiveOnByClassGroupAsync(TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(new Dictionary<Guid, int>());
        builder.Attendances
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, AttendanceCount>());

        var response = await builder.BuildListDay().ExecuteAsync(new ListDaySessionsQuery(null), CancellationToken.None);

        response.Value!.Select(session => session.ClassGroupId).ShouldBe([builder.ClassGroup.Id]);
    }
}
