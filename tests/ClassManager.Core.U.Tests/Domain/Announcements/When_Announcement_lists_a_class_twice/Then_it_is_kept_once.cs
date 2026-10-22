using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_lists_a_class_twice;

public sealed class Then_it_is_kept_once
{
    [Fact]
    public void Then_it_is_kept_once_Run()
    {
        var classGroupId = TestIds.Next();

        var announcement = Announcement.Create(
            "Cambia el horario", null, TestData.Now, TestData.Today, classGroupIds: [classGroupId, classGroupId]).Value!.WithId();

        announcement.ClassGroupIds.ShouldBe([classGroupId]);
    }
}
