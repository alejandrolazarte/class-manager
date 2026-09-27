using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.ImportExport;

namespace ClassManager.Api.Endpoints;

internal static class ImportExportEndpoints
{
    private const int MultipartOverheadInBytes = 64 * 1024;

    public static IEndpointRouteBuilder MapImportExportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var modules = endpoints.MapGroup(ApiRoutes.ImportExport + ApiRoutes.ImportModule)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);

        modules.MapGet(ApiRoutes.SchemaAction, GetSchemaAsync);
        modules.MapPost(ApiRoutes.PreviewAction, PreviewAsync)
            .DisableAntiforgery()
            .WithFormOptions(multipartBodyLengthLimit: ImportLimits.DefaultMaximumFileSizeInBytes + MultipartOverheadInBytes);
        modules.MapPost(ApiRoutes.ImportAction, ImportAsync)
            .DisableAntiforgery()
            .WithFormOptions(multipartBodyLengthLimit: ImportLimits.DefaultMaximumFileSizeInBytes + MultipartOverheadInBytes);

        return endpoints;
    }

    private static async Task<IResult> GetSchemaAsync(
        string module,
        IUseCase<GetImportSchemaQuery, ImportSchemaResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetImportSchemaQuery(module), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> PreviewAsync(
        string module,
        IFormFile file,
        IUseCase<PreviewImportCommand, ImportReport> useCase,
        CancellationToken cancellationToken)
    {
        await using var fileStream = file.OpenReadStream();
        var result = await useCase.ExecuteAsync(new PreviewImportCommand(module, fileStream), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ImportAsync(
        string module,
        IFormFile file,
        IUseCase<ImportFileCommand, ImportReport> useCase,
        CancellationToken cancellationToken)
    {
        await using var fileStream = file.OpenReadStream();
        var result = await useCase.ExecuteAsync(new ImportFileCommand(module, fileStream), cancellationToken);

        return result.ToOkResult();
    }
}
