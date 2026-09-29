using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListDaySessions_with_substitute;

public sealed class Then_substitute_and_usual_instructor_are_shown
{
    [Fact]
    public async Task Then_substitute_and_usual_instructor_are_shown_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var session = ClassSession.Create(builder.ClassGroup.Id, TestData.Today, TestData.Now);
        session.AssignSubstitute(builder.Substitute.Id, builder.Instructor.Id);
        builder.ClassGroups.Setup(repository => repository.ListActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([builder.ClassGroup]);
        builder.Sessions.Setup(repository => repository.ListByDateAsync(TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync([session]);
        builder.Enrollments.Setup(repository => repository.CountActiveOnByClassGroupAsync(TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(new Dictionary<Guid, int>());
        builder.Attendances
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, AttendanceCount>());

        var response = await builder.BuildListDay().ExecuteAsync(new ListDaySessionsQuery(null), CancellationToken.None);

        response.Value!.Single().InstructorFullName.ShouldBe(builder.Substitute.FullName);
        response.Value![0].OriginalInstructorFullName.ShouldBe(builder.Instructor.FullName);
    }
}
