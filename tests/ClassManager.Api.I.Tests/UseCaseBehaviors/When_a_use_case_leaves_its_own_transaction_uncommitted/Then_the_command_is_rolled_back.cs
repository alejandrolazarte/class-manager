using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_use_case_leaves_its_own_transaction_uncommitted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_command_is_rolled_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_command_is_rolled_back_Run()
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
                return UseCaseBehaviorProbe.Failed();
            });
        }

        (await UseCaseBehaviorProbe.ClientExistsAsync(fixture, business.Business.Id, client.Id)).ShouldBeFalse();
    }
}
