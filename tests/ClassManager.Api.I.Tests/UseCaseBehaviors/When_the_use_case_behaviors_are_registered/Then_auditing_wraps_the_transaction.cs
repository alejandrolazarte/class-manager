using ClassManager.Core.UseCases;
using ClassManager.Infrastructure.Audit;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_the_use_case_behaviors_are_registered;

public sealed class Then_auditing_wraps_the_transaction
{
    [Fact]
    public void Then_auditing_wraps_the_transaction_Run()
    {
        var services = new ServiceCollection().AddUseCaseBehaviors();

        services
            .Where(descriptor => descriptor.ServiceType == typeof(IUseCaseBehavior<,>))
            .Select(descriptor => descriptor.ImplementationType)
            .ShouldBe([
                typeof(CommandAuditBehavior<,>),
                typeof(QueryAuditBehavior<,>),
                typeof(CommandTransactionBehavior<,>),
                typeof(QueryNoTrackingBehavior<,>),
            ]);
    }
}
