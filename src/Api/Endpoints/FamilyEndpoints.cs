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
        family.MapGet(ApiRoutes.FamilyShop, GetFamilyShopAsync);
        family.MapGet(ApiRoutes.FamilyOrders, ListFamilyOrdersAsync);
        family.MapPost(ApiRoutes.FamilyOrders, PlaceFamilyOrderAsync);
        family.MapPut(ApiRoutes.FamilyOrders + ApiRoutes.OrderById + ApiRoutes.Cancellation, CancelFamilyOrderAsync);

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

    private static async Task<IResult> GetFamilyShopAsync(
        IUseCase<GetFamilyShopQuery, FamilyShopResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyShopQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListFamilyOrdersAsync(
        IUseCase<ListFamilyOrdersQuery, IReadOnlyList<FamilyOrderResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListFamilyOrdersQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> PlaceFamilyOrderAsync(
        PlaceFamilyOrderCommand command,
        IUseCase<PlaceFamilyOrderCommand, FamilyOrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(order => TypedResults.Created($"{ApiRoutes.Family}{ApiRoutes.FamilyOrders}/{order.Id}", order));
    }

    private static async Task<IResult> CancelFamilyOrderAsync(
        Guid orderId,
        IUseCase<CancelFamilyOrderCommand, FamilyOrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelFamilyOrderCommand(orderId), cancellationToken);

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
