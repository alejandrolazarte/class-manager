using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_has_a_group_pack_and_a_private_lesson_pack;

public sealed class Then_each_pays_its_own_kind
{
    [Fact]
    public void Then_each_pays_its_own_kind_Run()
    {
        var groupPack = ClassPackTestData.Purchase(new DateOnly(2026, 9, 1), validityMonths: 1, name: "Aquagym");
        var privateLessonsPack = ClassPackTestData.Pack(name: "Particulares");
        privateLessonsPack.DefineLessons([], 45);
        var privateLessons = ClassPackTestData.Sell(privateLessonsPack, new DateOnly(2026, 9, 1));

        var balance = ClassBalance.Calculate(
            [groupPack, privateLessons],
            [ClassPackTestData.AttendedPrivateLesson(new DateOnly(2026, 9, 3))],
            new DateOnly(2026, 9, 6));

        balance.PaidClasses.Single().Purchase.ShouldBe(privateLessons);
    }
}
