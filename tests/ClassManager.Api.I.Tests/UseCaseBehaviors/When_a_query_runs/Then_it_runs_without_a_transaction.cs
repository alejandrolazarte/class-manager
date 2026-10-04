using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_query_runs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_runs_without_a_transaction(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_runs_without_a_transaction_Run()
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IDbContextTransaction? transactionDuringQuery = null;

        await UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeQuery(), () =>
        {
            transactionDuringQuery = context.Database.CurrentTransaction;
            return UseCaseBehaviorProbe.Succeeded();
        });

        transactionDuringQuery.ShouldBeNull();
    }
}
