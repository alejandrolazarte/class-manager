using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_AssignSubstitute_with_busy_substitute;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var substituteClassGroup = ClassGroup.Create(
            "Aquagym", builder.Substitute.Id, ClassSchedule.Create([TestData.Today.DayOfWeek], "18:30", 45).Value!, 10, null).Value!;
        builder.ClassGroups
            .Setup(repository => repository.ListActiveByInstructorAsync(builder.Substitute.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([substituteClassGroup]);

        var response = await builder.BuildAssignSubstitute().ExecuteAsync(
            new AssignSubstituteCommand(builder.ClassGroup.Id, TestData.Today, builder.Substitute.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.InstructorBusy);
        response.Error.Details[ClassGroupErrorCodes.ConflictingClassGroupIdDetail].ShouldBe(substituteClassGroup.Id);
    }
}
