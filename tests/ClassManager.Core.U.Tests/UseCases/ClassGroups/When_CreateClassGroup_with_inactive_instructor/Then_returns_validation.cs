using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_CreateClassGroup_with_inactive_instructor;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        builder.Instructor.Deactivate();

        var response = await builder.BuildCreate().ExecuteAsync(new CreateClassGroupCommand(builder.ValidDetails()), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(ClassGroupDetails.InstructorId));
    }
}
