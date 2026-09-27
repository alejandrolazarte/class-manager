using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_client_pays_per_class;

public sealed class Then_it_is_listed_with_its_class_balance
{
    [Fact]
    public async Task Then_it_is_listed_with_its_class_balance_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var clientId = builder.AddEnrolledClient("Ana Pérez");
        builder.SetPlan(clientId, BillingPlanKind.ClassPacks);
        builder.ClassBalancesByClient[clientId] = new ClassBalance([], [new AttendedClass(TestData.Today, TestData.StudentFullName, "Natación")]);

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.Clients.ShouldBeEmpty();
        var classPackClient = response.Value.ClassPackClients.Single();
        classPackClient.ClientId.ShouldBe(clientId);
        classPackClient.UnpaidClasses.ShouldBe(1);
    }
}
