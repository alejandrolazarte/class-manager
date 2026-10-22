using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassGroup_with_a_material_file_gets_another_one;

public sealed class Then_the_first_one_is_replaced
{
    [Fact]
    public void Then_the_first_one_is_replaced_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Monday], "18:00", 45).Value!;
        var classGroup = ClassGroup.Create("Natación adultos", TestIds.Next(), schedule, 8, null).Value!.WithId();
        var firstFile = Document.Create(
            "tenant/class-groups/first.pdf", new DocumentContent([1], "application/pdf", ".pdf"), DocumentVisibility.Public, TestData.Now).WithId();
        var secondFile = Document.Create(
            "tenant/class-groups/second.pdf", new DocumentContent([2], "application/pdf", ".pdf"), DocumentVisibility.Public, TestData.Now).WithId();
        classGroup.ShareMaterialFile(firstFile);

        var replacedFile = classGroup.ShareMaterialFile(secondFile);

        replacedFile.ShouldBe(firstFile);
        classGroup.MaterialFiles.Select(materialFile => materialFile.Document).ShouldBe([secondFile]);
    }
}
