using ClassManager.Core.UseCases;
using ClassManager.Infrastructure.Audit;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Backoffice.Api.I.Tests.UseCaseBehaviors.When_the_backoffice_resolves_the_behaviors_of_a_command;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_auditing_wraps_the_transaction(BackofficeApiFixture fixture)
{
    [Fact]
    public void Then_auditing_wraps_the_transaction_Run()
    {
        using var scope = fixture.ApiFactory.Services.CreateScope();

        var behaviors = scope.ServiceProvider.GetServices<IUseCaseBehavior<SampleCommand, bool>>();

        behaviors.Select(behavior => behavior.GetType()).ShouldBe([
            typeof(CommandAuditBehavior<SampleCommand, bool>),
            typeof(CommandTransactionBehavior<SampleCommand, bool>),
        ]);
    }
}
