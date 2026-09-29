using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_gets_a_substitute;

public sealed class Then_the_substitute_teaches_it
{
    [Fact]
    public void Then_the_substitute_teaches_it_Run()
    {
        var usualInstructorId = Guid.CreateVersion7();
        var substituteInstructorId = Guid.CreateVersion7();
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);

        session.AssignSubstitute(substituteInstructorId, usualInstructorId);

        session.EffectiveInstructorId(usualInstructorId).ShouldBe(substituteInstructorId);
    }
}
