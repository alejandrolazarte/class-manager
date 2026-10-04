using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Brands;

namespace ClassManager.Api.Endpoints;

internal static class BrandEndpoints
{
    private const int LogoUploadLimitInBytes = BrandLogo.MaximumSizeInBytes * 4;

    public static IEndpointRouteBuilder MapBrandEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Business + ApiRoutes.Brand, GetBrandAsync).RequirePermission(Permissions.Business.View);
        endpoints.MapGet(ApiRoutes.Business + ApiRoutes.BrandLogo, GetBrandLogoAsync).RequirePermission(Permissions.Business.View);
        endpoints.MapPut(ApiRoutes.Business + ApiRoutes.Brand, UpdateBrandAsync)
            .RequirePermission(Permissions.Business.Manage)
            .RequireFeature(Features.Brand);
        endpoints.MapPut(ApiRoutes.Business + ApiRoutes.BrandLogo, SetBrandLogoAsync)
            .RequirePermission(Permissions.Business.Manage)
            .RequireFeature(Features.Brand)
            .DisableAntiforgery()
            .WithFormOptions(multipartBodyLengthLimit: LogoUploadLimitInBytes);
        endpoints.MapDelete(ApiRoutes.Business + ApiRoutes.BrandLogo, RemoveBrandLogoAsync).RequirePermission(Permissions.Business.Manage);

        endpoints.MapGet(ApiRoutes.StudentApp + ApiRoutes.Brand, GetBrandAsync).RequireStudent();
        endpoints.MapGet(ApiRoutes.StudentApp + ApiRoutes.BrandLogo, GetBrandLogoAsync).RequireStudent();

        return endpoints;
    }

    private static async Task<IResult> GetBrandAsync(
        IUseCase<GetBrandQuery, BrandResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetBrandQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetBrandLogoAsync(
        IUseCase<GetBrandLogoQuery, BrandLogoFile> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetBrandLogoQuery(), cancellationToken);

        return result.ToHttpResult(logo => TypedResults.File(logo.Content, logo.ContentType, lastModified: logo.UpdatedAt));
    }

    private static async Task<IResult> UpdateBrandAsync(
        UpdateBrandCommand command,
        IUseCase<UpdateBrandCommand, BrandResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SetBrandLogoAsync(
        IFormFile file,
        IUseCase<SetBrandLogoCommand, BrandResponse> useCase,
        CancellationToken cancellationToken)
    {
        using var content = new MemoryStream();
        await file.CopyToAsync(content, cancellationToken);
        var result = await useCase.ExecuteAsync(new SetBrandLogoCommand(content.ToArray()), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RemoveBrandLogoAsync(
        IUseCase<RemoveBrandLogoCommand, BrandResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoveBrandLogoCommand(), cancellationToken);

        return result.ToOkResult();
    }
}
