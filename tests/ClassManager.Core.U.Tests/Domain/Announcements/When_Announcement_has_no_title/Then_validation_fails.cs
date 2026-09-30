using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_has_no_title;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create("  ", "Las clases se recuperan durante la semana.", TestData.Now);

        announcement.Error!.FieldName.ShouldBe(nameof(Announcement.Title));
    }
}
