using ClassManager.Backoffice.Core.Brands;
using ClassManager.Backoffice.Core.Operators;
using ClassManager.Backoffice.Core.UseCases;
using ClassManager.Backoffice.Infrastructure.Brands;
using ClassManager.Backoffice.Infrastructure.Operators;
using ClassManager.Backoffice.Infrastructure.Persistence;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Infrastructure;
using ClassManager.Infrastructure.Security;
using ClassManager.Security.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClassManager.Backoffice.Infrastructure;

public static class BackofficeInfrastructureServiceCollectionExtensions
{
    private const string MissingConnectionStringMessage = "Connection string 'BusinessDatabase' is not configured.";

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
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>()
                .GetConnectionString(InfrastructureServiceCollectionExtensions.DatabaseConnectionStringName)
                ?? throw new InvalidOperationException(MissingConnectionStringMessage);
            options.UseSqlServer(connectionString, BackofficeDbContext.ConfigureSqlServer);
        });
        services.AddScoped<IBackofficeLoginRepository, BackofficeLoginRepository>();
        services.AddScoped<IPlatformOperatorDirectory, PlatformOperatorDirectory>();
        services.AddScoped<IBrandDirectory, BrandDirectory>();
        services.AddScoped<IBrandSubscriptionRepository, BrandSubscriptionRepository>();
        services.Replace(ServiceDescriptor.Scoped<ITokenSubjectResolver, BackofficeTokenSubjectResolver>());

        services.AddUseCase<SignInOperatorCommand, OperatorSignInResponse, SignInOperatorUseCase>();
        services.AddUseCase<RefreshSessionCommand, TokenResponse, RefreshSessionUseCase>();
        services.AddUseCase<SignOutCommand, SignOutResponse, SignOutUseCase>();
        services.AddUseCase<ListBrandsQuery, BrandListResponse, ListBrandsUseCase>();
        services.AddUseCase<GetBrandQuery, BrandDetailResponse, GetBrandUseCase>();
        services.AddUseCase<GetSubscriptionCatalogQuery, SubscriptionCatalogResponse, GetSubscriptionCatalogUseCase>();
        services.AddUseCase<ChangeBrandPlanCommand, BrandPlanChangedResponse, ChangeBrandPlanUseCase>();
        services.AddUseCase<GiveBrandAddOnCommand, BrandAddOnResponse, GiveBrandAddOnUseCase>();
        services.AddUseCase<RemoveBrandAddOnCommand, BrandAddOnResponse, RemoveBrandAddOnUseCase>();
        services.AddUseCase<RestoreBrandAddOnCommand, BrandAddOnResponse, RestoreBrandAddOnUseCase>();

        return services;
    }
}
