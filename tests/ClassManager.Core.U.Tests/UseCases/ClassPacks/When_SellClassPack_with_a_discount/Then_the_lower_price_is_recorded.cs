using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_SellClassPack_with_a_discount;

public sealed class Then_the_lower_price_is_recorded
{
    [Fact]
    public async Task Then_the_lower_price_is_recorded_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        ClassPackPurchase? soldPurchase = null;
        builder.Purchases.Setup(repository => repository.Add(It.IsAny<ClassPackPurchase>())).Callback<ClassPackPurchase>(purchase => soldPurchase = purchase);

        var response = await builder.BuildSell().ExecuteAsync(
            new SellClassPackCommand(client.Id, builder.Pack.Id, 150m, null, PaymentMethod.Cash, null), CancellationToken.None);

        response.Value!.Price.ShouldBe(150m);
        soldPurchase!.PurchasedOn.ShouldBe(TestData.Today);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
