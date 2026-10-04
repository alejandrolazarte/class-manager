using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_command_runs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_runs_inside_a_transaction(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_runs_inside_a_transaction_Run()
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IDbContextTransaction? transactionDuringCommand = null;

        await UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeCommand(), () =>
        {
            transactionDuringCommand = context.Database.CurrentTransaction;
            return UseCaseBehaviorProbe.Succeeded();
        });

        transactionDuringCommand.ShouldNotBeNull();
    }
}
