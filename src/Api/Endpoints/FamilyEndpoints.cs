using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.Endpoints;

internal static class FamilyEndpoints
{
    public static IEndpointRouteBuilder MapFamilyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var family = endpoints.MapGroup(ApiRoutes.Family).RequireFamily();
        family.MapGet("/", GetFamilyHomeAsync);

        endpoints.MapPost(ApiRoutes.Clients + ApiRoutes.ClientById + ApiRoutes.AppInvitation, InviteFamilyAsync)
            .RequirePermission(Permissions.Students.Manage);

        return endpoints;
    }

    private static async Task<IResult> GetFamilyHomeAsync(
        IUseCase<GetFamilyHomeQuery, FamilyHomeResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyHomeQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> InviteFamilyAsync(
        Guid clientId,
        InviteFamilyRequest request,
        IUseCase<InviteFamilyCommand, FamilyInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToHttpResult(invitation => TypedResults.Created($"{ApiRoutes.Clients}/{clientId}", invitation));
    }
}
