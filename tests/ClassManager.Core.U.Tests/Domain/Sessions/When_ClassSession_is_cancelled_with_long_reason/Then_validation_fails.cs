using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_is_cancelled_with_long_reason;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);

        var cancel = session.Cancel(new string('x', ClassSession.CancellationReasonMaxLength + 1));

        cancel.Error!.FieldName.ShouldBe(nameof(ClassSession.CancellationReason));
    }
}
