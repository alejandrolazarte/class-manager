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
            .RequirePermission(Permissions.ImportExport.Run);

        modules.MapGet(ApiRoutes.SchemaAction, GetSchemaAsync);
        modules.MapGet(ApiRoutes.TemplateAction, GetTemplateAsync);
        modules.MapGet(ApiRoutes.ExportAction, ExportAsync);
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

    private static async Task<IResult> GetTemplateAsync(
        string module,
        IUseCase<GetImportTemplateQuery, ExportFile> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetImportTemplateQuery(module), cancellationToken);

        return result.ToHttpResult(ToFileResult);
    }

    private static async Task<IResult> ExportAsync(
        string module,
        IUseCase<ExportQuery, ExportFile> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ExportQuery(module), cancellationToken);

        return result.ToHttpResult(ToFileResult);
    }

    private static IResult ToFileResult(ExportFile file) =>
        TypedResults.File(file.Content, file.ContentType, file.FileName);

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
