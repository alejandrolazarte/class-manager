using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_command_throws;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_changes_are_rolled_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_changes_are_rolled_back_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = UseCaseBehaviorProbe.NewClient(business);
        await using (var scope = UseCaseBehaviorProbe.BusinessScope(fixture, business.Business.Id))
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await Should.ThrowAsync<InvalidOperationException>(() => UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeCommand(), async () =>
            {
                context.Clients.Add(client);
                await unitOfWork.SaveChangesAsync(CancellationToken.None);
                throw new InvalidOperationException(UseCaseBehaviorProbe.ThrownMessage);
            }));
        }

        (await UseCaseBehaviorProbe.ClientExistsAsync(fixture, business.Business.Id, client.Id)).ShouldBeFalse();
    }
}
