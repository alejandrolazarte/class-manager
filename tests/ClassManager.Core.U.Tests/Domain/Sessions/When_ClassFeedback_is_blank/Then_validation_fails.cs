using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassFeedback_is_blank;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var feedback = ClassFeedback.Create(ClassSession.Create(TestIds.Next(), TestData.Today, TestData.Now).WithId(), TestIds.Next(), TestIds.Next(), "   ", TestData.Now);

        feedback.Error!.Code.ShouldBe(SessionErrorCodes.FeedbackRequired);
    }
}
