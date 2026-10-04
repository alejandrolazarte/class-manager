using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_ClassBalanceService_monthly_client_books_a_pack_class;

public sealed class Then_the_booked_class_uses_the_pack
{
    [Fact]
    public async Task Then_the_booked_class_uses_the_pack_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var clientId = Guid.CreateVersion7();
        builder.ClientPurchases.Add(ClassPackPurchase.Sell(
            clientId, builder.Pack, null, TestData.Today, PaymentMethod.Cash, null, TestData.Today, TestData.Now).Value!);
        builder.Attend(clientId, TestData.Today);
        builder.ClientAttendedClasses.Add(new ClientAttendedClass(
            clientId, new AttendedClass(TestData.Today, TestData.StudentFullName, "Aquagym", IsPackBooking: true)));

        var balances = await builder.BuildBalanceService().CalculateAsync([clientId], CancellationToken.None);

        balances[clientId].Purchases.Single().UsedClasses.ShouldBe(1);
    }
}
