using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks;

internal static class ClassPackTestData
{
    public static readonly Guid ClientId = Guid.CreateVersion7();

    public static ClassPack Pack(int classCount = 4, decimal price = 80m, int? validityMonths = null, string name = "4 clases") =>
        ClassPack.Create(name, classCount, price, validityMonths, TestData.Now).Value!;

    public static ClassPackPurchase Purchase(DateOnly purchasedOn, int classCount = 4, int? validityMonths = null, string name = "4 clases") =>
        ClassPackPurchase.Sell(ClientId, Pack(classCount, 80m, validityMonths, name), null, purchasedOn, PaymentMethod.Cash, null, TestData.Today, TestData.Now).Value!;

    public static AttendedClass Attended(DateOnly date) => new(date, TestData.StudentFullName, "Natación inicial");
}
