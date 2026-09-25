using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_CreateClassGroup_with_busy_instructor;

public sealed class Then_returns_conflict_with_other_group
{
    [Fact]
    public async Task Then_returns_conflict_with_other_group_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var otherClassGroup = builder.ExistingClassGroup("18:30");
        builder.ClassGroups
            .Setup(repository => repository.ListActiveByInstructorAsync(builder.Instructor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherClassGroup]);

        var response = await builder.BuildCreate().ExecuteAsync(new CreateClassGroupCommand(builder.ValidDetails()), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.InstructorBusy);
        response.Error.Details[ClassGroupErrorCodes.ConflictingClassGroupIdDetail].ShouldBe(otherClassGroup.Id);
    }
}
