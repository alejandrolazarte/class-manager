using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RescheduleSession_with_busy_instructor;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var otherClassGroup = ClassGroup.Create(
            "Aquagym", builder.Instructor.Id, ClassSchedule.Create([TestData.Today.DayOfWeek], "19:15", 45).Value!, 10, null).Value!;
        builder.ClassGroups
            .Setup(repository => repository.ListActiveByInstructorAsync(builder.Instructor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([builder.ClassGroup, otherClassGroup]);

        var response = await builder.BuildReschedule().ExecuteAsync(
            new RescheduleSessionCommand(builder.ClassGroup.Id, TestData.Today, "19:00"), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.InstructorBusy);
        response.Error.Details[ClassGroupErrorCodes.ConflictingClassGroupIdDetail].ShouldBe(otherClassGroup.Id);
    }
}
