using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_command_returns_a_failure_after_saving;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_changes_are_kept(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_changes_are_kept_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = UseCaseBehaviorProbe.NewClient(business);
        await using (var scope = UseCaseBehaviorProbe.BusinessScope(fixture, business.Business.Id))
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeCommand(), async () =>
            {
                context.Clients.Add(client);
                await unitOfWork.SaveChangesAsync(CancellationToken.None);
                return UseCaseBehaviorProbe.Failed();
            });
        }

        (await UseCaseBehaviorProbe.ClientExistsAsync(fixture, business.Business.Id, client.Id)).ShouldBeTrue();
    }
}
