using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_ClassBalanceService_student_has_no_monthly_plan;

public sealed class Then_every_class_uses_classes
{
    [Fact]
    public async Task Then_every_class_uses_classes_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var clientId = builder.Client.Id;
        var currentMonth = BillingMonth.From(TestData.Today);
        builder.Attend(clientId, currentMonth.FirstDay.AddDays(-7));
        builder.Attend(clientId, currentMonth.FirstDay.AddDays(1));

        var balances = await builder.BuildBalanceService().CalculateAsync([clientId], CancellationToken.None);

        balances[clientId].UnpaidClasses.ShouldBe(2);
    }
}
