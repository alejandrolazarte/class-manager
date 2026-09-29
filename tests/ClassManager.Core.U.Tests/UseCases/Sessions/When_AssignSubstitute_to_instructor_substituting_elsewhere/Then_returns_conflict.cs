using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_AssignSubstitute_to_instructor_substituting_elsewhere;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var otherClassGroup = ClassGroup.Create(
            "Aquagym", Guid.CreateVersion7(), ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!, 10, null).Value!;
        var otherSession = ClassSession.Create(otherClassGroup.Id, TestData.Today, TestData.Now);
        otherSession.AssignSubstitute(builder.Substitute.Id, otherClassGroup.InstructorId);
        builder.ClassGroups.Setup(repository => repository.GetByIdAsync(otherClassGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otherClassGroup);
        builder.Sessions
            .Setup(repository => repository.ListSubstitutionsAsync(builder.Substitute.Id, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherSession]);
        builder.Sessions
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherSession]);

        var response = await builder.BuildAssignSubstitute().ExecuteAsync(
            new AssignSubstituteCommand(builder.ClassGroup.Id, TestData.Today, builder.Substitute.Id), CancellationToken.None);

        response.Error!.Details[ClassGroupErrorCodes.ConflictingClassGroupIdDetail].ShouldBe(otherClassGroup.Id);
    }
}
