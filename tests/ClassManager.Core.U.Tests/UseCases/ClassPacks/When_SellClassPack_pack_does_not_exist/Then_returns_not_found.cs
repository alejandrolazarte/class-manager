using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_SellClassPack_pack_does_not_exist;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var response = await builder.BuildSell().ExecuteAsync(
            new SellClassPackCommand(client.Id, Guid.CreateVersion7(), null, null, PaymentMethod.Cash, null), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.NotFound);
    }
}
