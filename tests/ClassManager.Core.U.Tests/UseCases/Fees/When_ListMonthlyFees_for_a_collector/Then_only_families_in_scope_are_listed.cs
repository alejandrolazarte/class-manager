using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_for_a_collector;

public sealed class Then_only_families_in_scope_are_listed
{
    [Fact]
    public async Task Then_only_families_in_scope_are_listed_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var ownFamilyId = builder.AddEnrolledClient("Ana Pérez");
        builder.AddEnrolledClient("Carla Gómez");
        builder.ActAsCollector(ownFamilyId);

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.Clients.Select(client => client.ClientId).ShouldBe([ownFamilyId]);
        response.Value.TotalDue.ShouldBe(FeeUseCaseBuilder.DefaultFee);
    }
}
