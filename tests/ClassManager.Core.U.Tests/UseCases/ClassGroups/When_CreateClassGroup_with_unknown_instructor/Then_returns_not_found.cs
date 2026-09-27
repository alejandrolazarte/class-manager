using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_CreateClassGroup_with_unknown_instructor;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var details = builder.ValidDetails() with { InstructorId = Guid.CreateVersion7() };

        var response = await builder.BuildCreate().ExecuteAsync(new CreateClassGroupCommand(details), CancellationToken.None);

        response.Error!.Code.ShouldBe(InstructorErrorCodes.NotFound);
    }
}
