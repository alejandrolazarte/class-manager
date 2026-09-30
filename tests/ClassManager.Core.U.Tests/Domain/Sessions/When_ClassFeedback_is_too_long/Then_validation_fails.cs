using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassFeedback_is_too_long;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var feedback = ClassFeedback.Create(
            Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7(), new string('x', ClassFeedback.TextMaxLength + 1), TestData.Now);

        feedback.Error!.FieldName.ShouldBe(nameof(ClassFeedback.Text));
    }
}
