using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_category_is_unknown;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create(
            "Feriado", null, TestData.Now, TestData.Today, category: (AnnouncementCategory)99);

        announcement.Error!.FieldName.ShouldBe(nameof(Announcement.Category));
    }
}
