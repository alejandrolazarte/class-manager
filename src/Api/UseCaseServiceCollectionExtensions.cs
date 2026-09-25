using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api;

internal static class UseCaseServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IUseCase<SignUpOwnerCommand, TokenResponse>, SignUpOwnerUseCase>();
        services.AddScoped<IUseCase<SignInCommand, TokenResponse>, SignInUseCase>();
        services.AddScoped<IUseCase<RefreshSessionCommand, TokenResponse>, RefreshSessionUseCase>();
        services.AddScoped<IUseCase<SignOutCommand, SignOutResponse>, SignOutUseCase>();
        services.AddScoped<IUseCase<GetCurrentBusinessQuery, BusinessResponse>, GetCurrentBusinessUseCase>();
        services.AddScoped<IUseCase<RegisterClientCommand, ClientResponse>, RegisterClientUseCase>();
        services.AddScoped<IUseCase<GetClientQuery, ClientResponse>, GetClientUseCase>();
        services.AddScoped<IUseCase<SearchClientsQuery, IReadOnlyList<ClientResponse>>, SearchClientsUseCase>();
        services.AddScoped<IUseCase<UpdateBusinessSettingsCommand, UpdateBusinessSettingsResponse>, UpdateBusinessSettingsUseCase>();

        return services;
    }
}
