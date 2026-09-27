using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetClientBillingPlan_client_does_not_exist;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new FeeUseCaseBuilder();

        var response = await builder.BuildSetClientPlan().ExecuteAsync(
            new SetClientBillingPlanCommand(Guid.CreateVersion7(), BillingPlanKind.ClassPacks, null, null), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.NotFound);
    }
}
