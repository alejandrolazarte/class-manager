using ClassManager.Backoffice.Api.Authorization;
using ClassManager.Backoffice.Api.ErrorHandling;
using ClassManager.Backoffice.Core.UseCases;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.UseCases;

namespace ClassManager.Backoffice.Api.Endpoints;

internal static class BrandEndpoints
{
    public static IEndpointRouteBuilder MapBrandEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BackofficeApiRoutes.Brands);

        group.MapGet(string.Empty, async (IUseCase<ListBrandsQuery, BrandListResponse> useCase, CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new ListBrandsQuery(), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.View);
        group.MapGet(BackofficeApiRoutes.BrandById, async (Guid brandId, IUseCase<GetBrandQuery, BrandDetailResponse> useCase, CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new GetBrandQuery(brandId), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.View);

        group.MapPut(BackofficeApiRoutes.BrandSubscription, async (
            Guid brandId,
            ChangeBrandPlanRequest request,
            IUseCase<ChangeBrandPlanCommand, BrandPlanChangedResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(
                new ChangeBrandPlanCommand(brandId, request.PlanCode, request.Price, request.Currency, request.Note, request.ExpiredOn),
                cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.Manage);
        group.MapPost(BackofficeApiRoutes.BrandAddOns, async (
            Guid brandId,
            GiveBrandAddOnRequest request,
            IUseCase<GiveBrandAddOnCommand, BrandAddOnResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(
                new GiveBrandAddOnCommand(brandId, request.FeatureCode, request.Price, request.Currency, request.Limit, request.ExpiredOn),
                cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.Manage);
        group.MapDelete(BackofficeApiRoutes.BrandAddOnById, async (
            Guid brandId,
            Guid addOnId,
            IUseCase<RemoveBrandAddOnCommand, BrandAddOnResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new RemoveBrandAddOnCommand(brandId, addOnId), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.Manage);
        group.MapPost(BackofficeApiRoutes.BrandAddOnRestore, async (
            Guid brandId,
            Guid addOnId,
            IUseCase<RestoreBrandAddOnCommand, BrandAddOnResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new RestoreBrandAddOnCommand(brandId, addOnId), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.Manage);

        endpoints.MapGet(BackofficeApiRoutes.SubscriptionCatalog, async (
            IUseCase<GetSubscriptionCatalogQuery, SubscriptionCatalogResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new GetSubscriptionCatalogQuery(), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.View);

        return endpoints;
    }
}

internal sealed record ChangeBrandPlanRequest(string? PlanCode, decimal Price, string? Currency, string? Note, DateOnly? ExpiredOn);

internal sealed record GiveBrandAddOnRequest(string? FeatureCode, decimal Price, string? Currency, int? Limit, DateOnly? ExpiredOn);
