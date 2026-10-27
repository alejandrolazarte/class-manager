using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_ListInstructorAgenda_for_an_instructor_outside_the_scope;

public sealed class Then_returns_forbidden
{
    [Fact]
    public async Task Then_returns_forbidden_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var accessScopes = new Mock<IAccessScopes>();
        accessScopes
            .Setup(scopes => scopes.ForInstructorsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(InstructorScope.Only(builder.OtherInstructor.Id));

        var response = await builder.BuildListAgenda(accessScopes.Object).ExecuteAsync(
            new ListInstructorAgendaQuery(builder.Instructor.Id, TestData.Today), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Forbidden);
    }
}
