using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_visible_until_has_passed;

public sealed class Then_it_is_hidden
{
    [Fact]
    public void Then_it_is_hidden_Run()
    {
        var announcement = Announcement.Create(
            "Feriado", null, TestData.Now, TestData.Today, visibleUntil: TestData.Today.AddDays(3)).Value!.WithId();

        announcement.IsVisibleOn(TestData.Today.AddDays(3)).ShouldBeTrue();
        announcement.IsVisibleOn(TestData.Today.AddDays(4)).ShouldBeFalse();
    }
}
