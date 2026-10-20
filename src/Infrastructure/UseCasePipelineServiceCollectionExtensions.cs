using ClassManager.Core.UseCases;
using ClassManager.Infrastructure.Audit;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Infrastructure;

public static class UseCasePipelineServiceCollectionExtensions
{
    public static IServiceCollection AddUseCaseBehaviors(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandAuditBehavior<,>));
        services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(QueryAuditBehavior<,>));
        services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandTransactionBehavior<,>));
        services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(QueryNoTrackingBehavior<,>));

        return services;
    }

    public static IServiceCollection AddUseCase<TCommand, TResponse, TUseCase>(this IServiceCollection services)
        where TUseCase : class, IUseCase<TCommand, TResponse>
    {
        services.AddScoped<TUseCase>();
        services.AddScoped<IUseCase<TCommand, TResponse>>(serviceProvider => new UseCasePipeline<TCommand, TResponse>(
            serviceProvider.GetRequiredService<TUseCase>(),
            serviceProvider.GetServices<IUseCaseBehavior<TCommand, TResponse>>()));

        return services;
    }
}
