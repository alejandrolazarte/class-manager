using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

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
        services.AddScoped<IUseCase<RegisterClientCommand, ClientDetailsResponse>, RegisterClientUseCase>();
        services.AddScoped<IUseCase<GetClientQuery, ClientDetailsResponse>, GetClientUseCase>();
        services.AddScoped<IUseCase<SearchClientsQuery, IReadOnlyList<ClientResponse>>, SearchClientsUseCase>();
        services.AddScoped<IUseCase<AddStudentCommand, StudentResponse>, AddStudentUseCase>();
        services.AddScoped<IUseCase<GetStudentQuery, StudentSummaryResponse>, GetStudentUseCase>();
        services.AddScoped<IUseCase<SearchStudentsQuery, IReadOnlyList<StudentSummaryResponse>>, SearchStudentsUseCase>();
        services.AddScoped<IUseCase<UpdateBusinessSettingsCommand, UpdateBusinessSettingsResponse>, UpdateBusinessSettingsUseCase>();

        return services;
    }
}
