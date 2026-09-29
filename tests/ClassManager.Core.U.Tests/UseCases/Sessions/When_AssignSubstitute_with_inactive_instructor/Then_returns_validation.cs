using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_AssignSubstitute_with_inactive_instructor;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new SessionUseCaseBuilder();
        builder.Substitute.Deactivate();

        var response = await builder.BuildAssignSubstitute().ExecuteAsync(
            new AssignSubstituteCommand(builder.ClassGroup.Id, TestData.Today, builder.Substitute.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(InstructorErrorCodes.Inactive);
    }
}
