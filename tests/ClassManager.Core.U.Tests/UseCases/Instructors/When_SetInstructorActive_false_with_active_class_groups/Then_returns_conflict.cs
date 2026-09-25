using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_SetInstructorActive_false_with_active_class_groups;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        builder.ClassGroups
            .Setup(repository => repository.CountActiveByInstructorAsync(builder.Instructor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        var useCase = new SetInstructorActiveUseCase(builder.Instructors.Object, builder.ClassGroups.Object, builder.UnitOfWork.Object);

        var response = await useCase.ExecuteAsync(new SetInstructorActiveCommand(builder.Instructor.Id, false), CancellationToken.None);

        response.Error!.Code.ShouldBe(InstructorErrorCodes.HasActiveClassGroups);
        builder.Instructor.IsActive.ShouldBeTrue();
    }
}
