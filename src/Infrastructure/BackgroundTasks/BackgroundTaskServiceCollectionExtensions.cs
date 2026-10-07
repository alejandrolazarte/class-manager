using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Infrastructure.BackgroundTasks;

public static class BackgroundTaskServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundTask<TCommand, THandler>(this IServiceCollection services, string typeName)
        where TCommand : BackgroundTaskCommand
        where THandler : class, IBackgroundTaskHandler<TCommand>
    {
        services.AddScoped<IBackgroundTaskHandler<TCommand>, THandler>();
        services.AddSingleton(new BackgroundTaskRegistration(
            typeName,
            typeof(TCommand),
            (serviceProvider, command, cancellationToken) =>
                serviceProvider.GetRequiredService<IBackgroundTaskHandler<TCommand>>().HandleAsync((TCommand)command, cancellationToken)));

        return services;
    }

    internal static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
    {
        services.AddSingleton<BackgroundTaskTypes>();
        services.AddSingleton<BackgroundTaskSignal>();
        services.AddSingleton<IBackgroundTaskRunner, BackgroundTaskRunner>();
        services.AddScoped<BackgroundTaskSignalInterceptor>();
        services.AddScoped<IBackgroundTaskOutbox, BackgroundTaskOutbox>();

        return services;
    }
}
