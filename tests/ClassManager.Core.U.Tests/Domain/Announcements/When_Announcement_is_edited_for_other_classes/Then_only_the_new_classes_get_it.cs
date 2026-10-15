using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_edited_for_other_classes;

public sealed class Then_only_the_new_classes_get_it
{
    [Fact]
    public void Then_only_the_new_classes_get_it_Run()
    {
        var keptClassGroupId = Guid.NewGuid();
        var removedClassGroupId = Guid.NewGuid();
        var addedClassGroupId = Guid.NewGuid();
        var announcement = Announcement.Create(
            "Cambia el horario", null, TestData.Now, TestData.Today, classGroupIds: [keptClassGroupId, removedClassGroupId]).Value!;

        announcement.Edit("Cambia el horario", null, AnnouncementCategory.News, [keptClassGroupId, addedClassGroupId], null, TestData.Today);

        announcement.ClassGroupIds.ShouldBe([keptClassGroupId, addedClassGroupId], ignoreOrder: true);
    }
}
