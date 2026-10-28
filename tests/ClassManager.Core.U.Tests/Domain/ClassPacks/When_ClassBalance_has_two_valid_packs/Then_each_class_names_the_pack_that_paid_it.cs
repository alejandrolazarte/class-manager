using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_has_two_valid_packs;

public sealed class Then_each_class_names_the_pack_that_paid_it
{
    [Fact]
    public void Then_each_class_names_the_pack_that_paid_it_Run()
    {
        var withoutExpiry = ClassPackTestData.Purchase(new DateOnly(2026, 9, 1), classCount: 8, name: "8 clases");
        var expiringSoon = ClassPackTestData.Purchase(new DateOnly(2026, 9, 2), classCount: 2, validityMonths: 1);
        IReadOnlyList<AttendedClass> attendedClasses =
        [
            ClassPackTestData.Attended(new DateOnly(2026, 9, 3)),
            ClassPackTestData.Attended(new DateOnly(2026, 9, 4)),
            ClassPackTestData.Attended(new DateOnly(2026, 9, 5)),
        ];

        var balance = ClassBalance.Calculate([withoutExpiry, expiringSoon], attendedClasses, new DateOnly(2026, 9, 6));

        balance.PaidClasses.Select(paidClass => paidClass.Purchase).ShouldBe([expiringSoon, expiringSoon, withoutExpiry]);
    }
}
