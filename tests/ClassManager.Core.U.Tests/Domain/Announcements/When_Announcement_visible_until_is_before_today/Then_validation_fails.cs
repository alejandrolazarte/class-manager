using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_visible_until_is_before_today;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create(
            "Feriado", null, TestData.Now, TestData.Today, visibleUntil: TestData.Today.AddDays(-1));

        announcement.Error!.FieldName.ShouldBe(nameof(Announcement.VisibleUntil));
    }
}
