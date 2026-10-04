using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_use_case_commits_its_own_transaction;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_command_is_committed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_command_is_committed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = UseCaseBehaviorProbe.NewClient(business);
        await using (var scope = UseCaseBehaviorProbe.BusinessScope(fixture, business.Business.Id))
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeCommand(), async () =>
            {
                await using var transaction = await unitOfWork.BeginTransactionAsync(CancellationToken.None);
                context.Clients.Add(client);
                await unitOfWork.SaveChangesAsync(CancellationToken.None);
                await transaction.CommitAsync(CancellationToken.None);
                return await UseCaseBehaviorProbe.Succeeded();
            });
        }

        (await UseCaseBehaviorProbe.ClientExistsAsync(fixture, business.Business.Id, client.Id)).ShouldBeTrue();
    }
}
