using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_attendances_exceed_the_packs;

public sealed class Then_the_extra_classes_are_unpaid
{
    [Fact]
    public void Then_the_extra_classes_are_unpaid_Run()
    {
        var purchase = ClassPackTestData.Purchase(new DateOnly(2026, 9, 1), classCount: 1);
        AttendedClass[] attendedClasses =
        [
            ClassPackTestData.Attended(new DateOnly(2026, 9, 5)),
            ClassPackTestData.Attended(new DateOnly(2026, 9, 12)),
            ClassPackTestData.Attended(new DateOnly(2026, 9, 19)),
        ];

        var balance = ClassBalance.Calculate([purchase], attendedClasses, new DateOnly(2026, 9, 20));

        balance.UnpaidClasses.ShouldBe(2);
        balance.UnpaidAttendances.Select(attended => attended.Date).ShouldBe([new DateOnly(2026, 9, 12), new DateOnly(2026, 9, 19)]);
        balance.Purchases.Single().Status.ShouldBe(ClassPackPurchaseStatus.UsedUp);
    }
}
