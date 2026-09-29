using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_AssignSubstitute_first_change_of_the_day;

public sealed class Then_session_is_added_with_substitute
{
    [Fact]
    public async Task Then_session_is_added_with_substitute_Run()
    {
        var builder = new SessionUseCaseBuilder();
        ClassSession? addedSession = null;
        builder.Sessions.Setup(repository => repository.Add(It.IsAny<ClassSession>())).Callback<ClassSession>(session => addedSession = session);

        var response = await builder.BuildAssignSubstitute().ExecuteAsync(
            new AssignSubstituteCommand(builder.ClassGroup.Id, TestData.Today, builder.Substitute.Id), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        addedSession!.SubstituteInstructorId.ShouldBe(builder.Substitute.Id);
    }
}
