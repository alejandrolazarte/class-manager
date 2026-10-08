using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.Endpoints;

internal static class ClassGroupEndpoints
{
    private const int MultipartOverheadInBytes = 64 * 1024;
    private const int MaterialUploadLimitInBytes = ClassMaterialFile.MaximumSizeInBytes + MultipartOverheadInBytes;

    public static IEndpointRouteBuilder MapClassGroupEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var classGroups = endpoints.MapGroup(ApiRoutes.ClassGroups);

        classGroups.MapGet("/", ListClassGroupsAsync).RequirePermission(Permissions.ClassGroups.ViewAll, Permissions.ClassGroups.ViewOwn);
        classGroups.MapGet(ApiRoutes.ClassGroupById, GetClassGroupAsync).RequirePermission(Permissions.ClassGroups.ViewAll, Permissions.ClassGroups.ViewOwn);
        classGroups.MapPost("/", CreateClassGroupAsync).RequirePermission(Permissions.ClassGroups.Manage);
        classGroups.MapPut(ApiRoutes.ClassGroupById, UpdateClassGroupAsync).RequirePermission(Permissions.ClassGroups.Manage);
        classGroups.MapPut(ApiRoutes.ClassGroupById + ApiRoutes.Active, SetClassGroupActiveAsync).RequirePermission(Permissions.ClassGroups.Manage);
        classGroups.MapPost(ApiRoutes.ClassGroupById + ApiRoutes.Material, UploadClassMaterialFileAsync)
            .RequirePermission(Permissions.ClassGroups.Manage)
            .DisableAntiforgery()
            .WithFormOptions(multipartBodyLengthLimit: MaterialUploadLimitInBytes);
        classGroups.MapDelete(ApiRoutes.ClassGroupById + ApiRoutes.Material, RemoveClassMaterialFileAsync)
            .RequirePermission(Permissions.ClassGroups.Manage);

        return endpoints;
    }

    private static async Task<IResult> ListClassGroupsAsync(
        bool? includeInactive,
        IUseCase<ListClassGroupsQuery, IReadOnlyList<ClassGroupResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListClassGroupsQuery(includeInactive ?? false), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetClassGroupAsync(
        Guid classGroupId,
        IUseCase<GetClassGroupQuery, ClassGroupResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetClassGroupQuery(classGroupId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateClassGroupAsync(
        ClassGroupDetails details,
        IUseCase<CreateClassGroupCommand, ClassGroupResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CreateClassGroupCommand(details), cancellationToken);

        return result.ToHttpResult(classGroup => TypedResults.Created($"{ApiRoutes.ClassGroups}/{classGroup.Id}", classGroup));
    }

    private static async Task<IResult> UpdateClassGroupAsync(
        Guid classGroupId,
        ClassGroupDetails details,
        IUseCase<UpdateClassGroupCommand, ClassGroupResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new UpdateClassGroupCommand(classGroupId, details), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SetClassGroupActiveAsync(
        Guid classGroupId,
        SetActiveRequest request,
        IUseCase<SetClassGroupActiveCommand, ClassGroupResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetClassGroupActiveCommand(classGroupId, request.IsActive), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> UploadClassMaterialFileAsync(
        Guid classGroupId,
        IFormFile file,
        IUseCase<UploadClassMaterialFileCommand, ClassGroupResponse> useCase,
        CancellationToken cancellationToken)
    {
        var content = await ImageUploads.ReadAsync(file, cancellationToken);
        var result = await useCase.ExecuteAsync(new UploadClassMaterialFileCommand(classGroupId, content), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RemoveClassMaterialFileAsync(
        Guid classGroupId,
        IUseCase<RemoveClassMaterialFileCommand, ClassGroupResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoveClassMaterialFileCommand(classGroupId), cancellationToken);

        return result.ToOkResult();
    }
}
