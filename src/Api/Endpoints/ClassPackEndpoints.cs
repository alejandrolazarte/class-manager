using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.Endpoints;

internal static class ClassPackEndpoints
{
    public static IEndpointRouteBuilder MapClassPackEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var classPacks = endpoints.MapGroup(ApiRoutes.ClassPacks);
        classPacks.MapGet("/", ListClassPacksAsync).RequirePermission(Permissions.ClassPacks.View);
        classPacks.MapPost("/", CreateClassPackAsync).RequirePermission(Permissions.ClassPacks.Manage);
        classPacks.MapPut(ApiRoutes.ClassPackById, UpdateClassPackAsync).RequirePermission(Permissions.ClassPacks.Manage);
        classPacks.MapPut(ApiRoutes.ClassPackById + ApiRoutes.Active, SetClassPackActiveAsync).RequirePermission(Permissions.ClassPacks.Manage);

        var clients = endpoints.MapGroup(ApiRoutes.Clients);
        clients.MapPost(ApiRoutes.ClientById + ApiRoutes.ClassPackPurchasesSegment, SellClassPackAsync)
            .RequirePermission(Permissions.ClassPacks.Sell);
        clients.MapGet(ApiRoutes.ClientById + ApiRoutes.ClassBalance, GetClientClassBalanceAsync)
            .RequirePermission(Permissions.ClassPacks.View);

        endpoints.MapGroup(ApiRoutes.ClassPackPurchases).MapDelete(ApiRoutes.ClassPackPurchaseById, DeleteClassPackPurchaseAsync)
            .RequirePermission(Permissions.ClassPacks.Sell);

        return endpoints;
    }

    private static async Task<IResult> ListClassPacksAsync(
        bool? includeInactive,
        IUseCase<ListClassPacksQuery, IReadOnlyList<ClassPackResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListClassPacksQuery(includeInactive ?? false), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateClassPackAsync(
        CreateClassPackCommand command,
        IUseCase<CreateClassPackCommand, ClassPackResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(classPack => TypedResults.Created($"{ApiRoutes.ClassPacks}/{classPack.Id}", classPack));
    }

    private static async Task<IResult> UpdateClassPackAsync(
        Guid classPackId,
        UpdateClassPackRequest request,
        IUseCase<UpdateClassPackCommand, ClassPackResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(classPackId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SetClassPackActiveAsync(
        Guid classPackId,
        SetActiveRequest request,
        IUseCase<SetClassPackActiveCommand, ClassPackResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetClassPackActiveCommand(classPackId, request.IsActive), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SellClassPackAsync(
        Guid clientId,
        SellClassPackRequest request,
        IUseCase<SellClassPackCommand, ClassPackPurchaseResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToHttpResult(purchase =>
            TypedResults.Created($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.ClassBalance}", purchase));
    }

    private static async Task<IResult> GetClientClassBalanceAsync(
        Guid clientId,
        IUseCase<GetClientClassBalanceQuery, ClassBalanceResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetClientClassBalanceQuery(clientId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeleteClassPackPurchaseAsync(
        Guid purchaseId,
        IUseCase<DeleteClassPackPurchaseCommand, ClassPackPurchaseResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeleteClassPackPurchaseCommand(purchaseId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
