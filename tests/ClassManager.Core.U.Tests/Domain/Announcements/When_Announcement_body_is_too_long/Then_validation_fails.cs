using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_body_is_too_long;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create(
            "Feriado", new string('x', Announcement.BodyMaxLength + 1), TestData.Now);

        announcement.Error!.FieldName.ShouldBe(nameof(Announcement.Body));
    }
}
