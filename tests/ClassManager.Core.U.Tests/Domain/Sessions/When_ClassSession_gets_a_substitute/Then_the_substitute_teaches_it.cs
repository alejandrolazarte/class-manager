using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_gets_a_substitute;

public sealed class Then_the_substitute_teaches_it
{
    [Fact]
    public void Then_the_substitute_teaches_it_Run()
    {
        var usualInstructorId = TestIds.Next();
        var substituteInstructorId = TestIds.Next();
        var session = ClassSession.Create(TestIds.Next(), TestData.Today, TestData.Now).WithId();

        session.AssignSubstitute(substituteInstructorId, usualInstructorId, TestData.Now);

        session.EffectiveInstructorId(usualInstructorId).ShouldBe(substituteInstructorId);
    }
}
