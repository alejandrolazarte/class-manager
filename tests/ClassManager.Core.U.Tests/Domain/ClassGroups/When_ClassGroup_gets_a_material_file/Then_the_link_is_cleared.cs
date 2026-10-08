using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassGroup_gets_a_material_file;

public sealed class Then_the_link_is_cleared
{
    [Fact]
    public void Then_the_link_is_cleared_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Monday], "18:00", 45).Value!;
        var classGroup = ClassGroup.Create("Natación adultos", Guid.CreateVersion7(), schedule, 8, null).Value!;
        classGroup.ShareMaterial("https://example.com/adultos.pdf");
        var file = Document.Create(
            Guid.CreateVersion7(), "tenant/class-groups/file.pdf", new DocumentContent([1], "application/pdf", ".pdf"), DocumentVisibility.Public, TestData.Now);

        classGroup.ShareMaterialFile(file);

        (classGroup.MaterialUrl, classGroup.MaterialDocument).ShouldBe((null, file));
    }
}
