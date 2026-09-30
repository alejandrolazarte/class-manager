using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassFeedback_is_rewritten;

public sealed class Then_the_new_text_and_instructor_are_kept
{
    [Fact]
    public void Then_the_new_text_and_instructor_are_kept_Run()
    {
        var feedback = ClassFeedback.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7(), "Buena clase", TestData.Now).Value!;
        var substituteId = Guid.CreateVersion7();

        var rewrite = feedback.Rewrite("  Mejoró la patada  ", substituteId, TestData.Now.AddHours(1));

        rewrite.IsSuccess.ShouldBeTrue();
        feedback.Text.ShouldBe("Mejoró la patada");
        feedback.InstructorId.ShouldBe(substituteId);
        feedback.UpdatedAt.ShouldBe(TestData.Now.AddHours(1));
    }
}
