using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_business_charges_with_class_packs;

public sealed class Then_clients_without_plan_are_listed_by_their_classes
{
    [Fact]
    public async Task Then_clients_without_plan_are_listed_by_their_classes_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.AddDefaultFeeChange(builder.CurrentMonth, FeeUseCaseBuilder.DefaultFee, BusinessBillingMode.ClassPacks);
        var classPackClientId = builder.AddEnrolledClient("Ana Pérez");
        var customFeeClientId = builder.AddEnrolledClient("Sofía Martínez", 20000m);
        builder.ClassBalancesByClient[classPackClientId] = new ClassBalance([], [], []);

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.ClassPackClients.Single().ClientId.ShouldBe(classPackClientId);
        response.Value.Clients.Single().ClientId.ShouldBe(customFeeClientId);
        response.Value.BillingMode.ShouldBe(BusinessBillingMode.ClassPacks);
    }
}
