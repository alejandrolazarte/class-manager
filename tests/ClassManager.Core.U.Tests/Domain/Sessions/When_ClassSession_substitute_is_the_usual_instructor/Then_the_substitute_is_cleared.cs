using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_substitute_is_the_usual_instructor;

public sealed class Then_the_substitute_is_cleared
{
    [Fact]
    public void Then_the_substitute_is_cleared_Run()
    {
        var usualInstructorId = Guid.CreateVersion7();
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);
        session.AssignSubstitute(Guid.CreateVersion7(), usualInstructorId, TestData.Now);

        session.AssignSubstitute(usualInstructorId, usualInstructorId, TestData.Now);

        session.SubstituteInstructorId.ShouldBeNull();
    }
}
