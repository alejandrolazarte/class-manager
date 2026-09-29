using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.Endpoints;

internal static class ClassGroupEndpoints
{
    public static IEndpointRouteBuilder MapClassGroupEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var classGroups = endpoints.MapGroup(ApiRoutes.ClassGroups);

        classGroups.MapGet("/", ListClassGroupsAsync).RequirePermission(Permissions.ClassGroups.ViewAll, Permissions.ClassGroups.ViewOwn);
        classGroups.MapGet(ApiRoutes.ClassGroupById, GetClassGroupAsync).RequirePermission(Permissions.ClassGroups.ViewAll, Permissions.ClassGroups.ViewOwn);
        classGroups.MapPost("/", CreateClassGroupAsync).RequirePermission(Permissions.ClassGroups.Manage);
        classGroups.MapPut(ApiRoutes.ClassGroupById, UpdateClassGroupAsync).RequirePermission(Permissions.ClassGroups.Manage);
        classGroups.MapPut(ApiRoutes.ClassGroupById + ApiRoutes.Active, SetClassGroupActiveAsync).RequirePermission(Permissions.ClassGroups.Manage);

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
}
