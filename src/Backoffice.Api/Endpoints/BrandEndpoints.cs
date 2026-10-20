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
        var group = endpoints.MapGroup(BackofficeApiRoutes.Brands)
            .RequirePlatformPermission(Permissions.Platform.View);

        group.MapGet(string.Empty, async (IUseCase<ListBrandsQuery, BrandListResponse> useCase, CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new ListBrandsQuery(), cancellationToken)).ToOkResult());
        group.MapGet(BackofficeApiRoutes.BrandById, async (Guid brandId, IUseCase<GetBrandQuery, BrandDetailResponse> useCase, CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new GetBrandQuery(brandId), cancellationToken)).ToOkResult());

        return endpoints;
    }
}
