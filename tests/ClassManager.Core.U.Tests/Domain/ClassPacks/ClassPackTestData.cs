using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks;

internal static class ClassPackTestData
{
    public static readonly long ClientId = TestIds.Next();
    public static readonly long ClassGroupId = TestIds.Next();

    public static ClassPack Pack(int classCount = 4, decimal price = 80m, int? validityMonths = null, string name = "4 clases")
    {
        var pack = ClassPack.Create(name, classCount, price, validityMonths, TestData.Now).Value!.WithId();
        pack.DefineLessons([], null, coversAnyGroupClass: true);
        return pack;
    }

    public static ClassPackPurchase Purchase(DateOnly purchasedOn, int classCount = 4, int? validityMonths = null, string name = "4 clases") =>
        Sell(Pack(classCount, 80m, validityMonths, name), purchasedOn);

    public static ClassPackPurchase Sell(ClassPack pack, DateOnly purchasedOn) =>
        ClassPackPurchase.Sell(ClientId, pack, null, TestData.CurrencyCode, purchasedOn, PaymentMethod.Cash, null, TestData.Today, TestData.Now).Value!.WithId();

    public static AttendedClass Attended(DateOnly date, long? classGroupId = null) =>
        new(date, TestData.StudentFullName, "Natación inicial", ClassGroupId: classGroupId ?? ClassGroupId);

    public static AttendedClass AttendedPrivateLesson(DateOnly date) =>
        new(date, TestData.StudentFullName, TestData.InstructorFullName, IsPrivateLesson: true);
}
