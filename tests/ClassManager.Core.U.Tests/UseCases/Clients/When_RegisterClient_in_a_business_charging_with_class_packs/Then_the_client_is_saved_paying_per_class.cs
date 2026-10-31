using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_in_a_business_charging_with_class_packs;

public sealed class Then_the_client_is_saved_paying_per_class
{
    [Fact]
    public async Task Then_the_client_is_saved_paying_per_class_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        builder.FeeSchedule
            .Setup(repository => repository.ListDefaultFeeChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId()]);
        Client? addedClient = null;
        builder.Clients.Setup(repository => repository.Add(It.IsAny<Client>())).Callback<Client>(client => addedClient = client);

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.FeeSchedule.Verify(
            repository => repository.Add(It.Is<ClientBillingPlanChange>(change => change.Client == addedClient && change.Kind == BillingPlanKind.ClassPacks)),
            Times.Once);
        response.Value!.BillingPlanChanges.Single().Kind.ShouldBe(BillingPlanKind.ClassPacks);
    }
}
