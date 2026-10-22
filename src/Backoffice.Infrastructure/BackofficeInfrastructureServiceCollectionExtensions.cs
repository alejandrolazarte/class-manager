using ClassManager.Backoffice.Core.Brands;
using ClassManager.Backoffice.Core.Operators;
using ClassManager.Backoffice.Core.UseCases;
using ClassManager.Backoffice.Infrastructure.Brands;
using ClassManager.Backoffice.Infrastructure.Logs;
using ClassManager.Backoffice.Infrastructure.Operators;
using ClassManager.Backoffice.Infrastructure.Persistence;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Infrastructure;
using ClassManager.Infrastructure.Audit;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.Security;
using ClassManager.Security.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClassManager.Backoffice.Infrastructure;

public static class BackofficeInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddBackofficeInfrastructure(this IServiceCollection services)
    {
        services
            .AddInfrastructure()
            .AddSessionClaims()
            .AddUseCaseBehaviors();

        services.AddOptions<BackofficeOptions>()
            .BindConfiguration(BackofficeOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddDbContext<BackofficeDbContext>((serviceProvider, options) =>
            options.UseSqlServer(
                serviceProvider.GetRequiredService<AppDbContext>().Database.GetDbConnection(),
                BackofficeDbContext.ConfigureSqlServer));
        services.AddScoped<ISharedTransactionContext>(serviceProvider => serviceProvider.GetRequiredService<BackofficeDbContext>());
        services.AddScoped<IBackofficeLoginRepository, BackofficeLoginRepository>();
        services.AddScoped<IPlatformOperatorDirectory, PlatformOperatorDirectory>();
        services.AddScoped<IBackofficeSessionService, BackofficeSessionService>();
        services.AddAuditLogReader();
        services.Replace(ServiceDescriptor.Singleton(AuditSource.Backoffice));
        services.AddScoped<IBrandDirectory, BrandDirectory>();
        services.AddScoped<IBrandSubscriptionRepository, BrandSubscriptionRepository>();
        services.Replace(ServiceDescriptor.Scoped<ITokenSubjectResolver, BackofficeTokenSubjectResolver>());

        services.AddUseCase<SignInOperatorCommand, OperatorSignInResponse, SignInOperatorUseCase>();
        services.AddUseCase<RefreshSessionCommand, TokenResponse, RefreshSessionUseCase>();
        services.AddUseCase<SignOutCommand, SignOutResponse, SignOutUseCase>();
        services.AddUseCase<ListBrandsQuery, BrandListResponse, ListBrandsUseCase>();
        services.AddUseCase<GetBrandQuery, BrandDetailResponse, GetBrandUseCase>();
        services.AddUseCase<ListExpiringAddOnsQuery, ExpiringAddOnListResponse, ListExpiringAddOnsUseCase>();
        services.AddUseCase<GetSubscriptionCatalogQuery, SubscriptionCatalogResponse, GetSubscriptionCatalogUseCase>();
        services.AddUseCase<ChangeBrandPlanCommand, BrandPlanChangedResponse, ChangeBrandPlanUseCase>();
        services.AddUseCase<GiveBrandAddOnCommand, BrandAddOnResponse, GiveBrandAddOnUseCase>();
        services.AddUseCase<RemoveBrandAddOnCommand, BrandAddOnResponse, RemoveBrandAddOnUseCase>();
        services.AddUseCase<RestoreBrandAddOnCommand, BrandAddOnResponse, RestoreBrandAddOnUseCase>();
        services.AddUseCase<ListOperatorsQuery, OperatorListResponse, ListOperatorsUseCase>();
        services.AddUseCase<EnableOperatorCommand, OperatorLoginResponse, EnableOperatorUseCase>();
        services.AddUseCase<DisableOperatorCommand, OperatorLoginResponse, DisableOperatorUseCase>();
        services.AddUseCase<ListCommandLogQuery, CommandLogResponse, ListCommandLogUseCase>();
        services.AddUseCase<ListSecurityEventsQuery, SecurityEventListResponse, ListSecurityEventsUseCase>();

        return services;
    }
}
