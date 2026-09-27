using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_SellClassPack_deducts_a_trial_of_another_family;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var client = Client.Create(TestData.ClientFullName, TestData.PhoneNumber(), null, null, TestData.Now).Value!;
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        builder.PrivateLessons
            .Setup(repository => repository.ListPaidTrialsByClientAsync(client.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var response = await builder.BuildSell().ExecuteAsync(
            new SellClassPackCommand(client.Id, builder.Pack.Id, 425m, TestData.Today, PaymentMethod.Cash, null, Guid.CreateVersion7()),
            CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassPackErrorCodes.TrialNotDeductible);
    }
}
