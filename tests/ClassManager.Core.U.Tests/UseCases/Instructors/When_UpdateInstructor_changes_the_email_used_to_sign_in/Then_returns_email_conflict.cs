using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_UpdateInstructor_changes_the_email_used_to_sign_in;

public sealed class Then_returns_email_conflict
{
    private const string OtherEmail = "laura.nueva@example.com";

    [Fact]
    public async Task Then_returns_email_conflict_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        builder.WithLinkedMember();

        var response = await builder.BuildUpdate().ExecuteAsync(
            new UpdateInstructorCommand(builder.Instructor.Id, TestData.InstructorFullName, OtherEmail), CancellationToken.None);

        response.Error!.Code.ShouldBe(InstructorErrorCodes.EmailUsedToSignIn);
    }
}
