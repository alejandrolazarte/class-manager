using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Api.Endpoints;

internal static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var roles = endpoints.MapGroup(ApiRoutes.Roles);
        roles.MapGet("/", ListAsync).RequirePermission(Permissions.Members.View);
        roles.MapPost("/", CreateAsync).RequirePermission(Permissions.Roles.Manage);
        roles.MapPut(ApiRoutes.RoleById, UpdateAsync).RequirePermission(Permissions.Roles.Manage);
        roles.MapDelete(ApiRoutes.RoleById, DeleteAsync).RequirePermission(Permissions.Roles.Manage);

        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        IUseCase<ListRolesQuery, IReadOnlyList<RoleResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListRolesQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateAsync(
        CreateRoleCommand command,
        IUseCase<CreateRoleCommand, RoleResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(role => TypedResults.Created($"{ApiRoutes.Roles}/{role.Id}", role));
    }

    private static async Task<IResult> UpdateAsync(
        Guid roleId,
        UpdateRoleRequest request,
        IUseCase<UpdateRoleCommand, RoleResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new UpdateRoleCommand(roleId, request.Name, request.Permissions), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeleteAsync(
        Guid roleId,
        IUseCase<DeleteRoleCommand, DeletedRoleResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeleteRoleCommand(roleId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
