using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_for_some_classes;

public sealed class Then_only_students_of_those_classes_get_it
{
    [Fact]
    public void Then_only_students_of_those_classes_get_it_Run()
    {
        var targetClassGroupId = TestIds.Next();
        var announcement = Announcement.Create(
            "Cambia el horario", null, TestData.Now, TestData.Today, classGroupIds: [targetClassGroupId]).Value!.WithId();

        announcement.IsFor([TestIds.Next(), targetClassGroupId]).ShouldBeTrue();
        announcement.IsFor([TestIds.Next()]).ShouldBeFalse();
        announcement.IsFor([]).ShouldBeFalse();
        announcement.IsForEveryone.ShouldBeFalse();
    }
}
