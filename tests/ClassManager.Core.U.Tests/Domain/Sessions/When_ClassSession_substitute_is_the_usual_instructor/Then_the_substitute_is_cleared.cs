using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_substitute_is_the_usual_instructor;

public sealed class Then_the_substitute_is_cleared
{
    [Fact]
    public void Then_the_substitute_is_cleared_Run()
    {
        var usualInstructorId = TestIds.Next();
        var session = ClassSession.Create(TestIds.Next(), TestData.Today, TestData.Now).WithId();
        session.AssignSubstitute(TestIds.Next(), usualInstructorId, TestData.Now);

        session.AssignSubstitute(usualInstructorId, usualInstructorId, TestData.Now);

        session.SubstituteInstructorId.ShouldBeNull();
    }
}
