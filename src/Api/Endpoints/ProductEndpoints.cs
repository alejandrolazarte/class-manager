using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.Endpoints;

internal static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var products = endpoints.MapGroup(ApiRoutes.Products);
        products.MapGet("/", ListProductsAsync).RequirePermission(Permissions.Products.View, Permissions.Orders.Manage);
        products.MapPost("/", CreateProductAsync).RequirePermission(Permissions.Products.Manage);
        products.MapPut(ApiRoutes.ProductById, UpdateProductAsync).RequirePermission(Permissions.Products.Manage);
        products.MapPut(ApiRoutes.ProductById + ApiRoutes.Active, SetProductActiveAsync).RequirePermission(Permissions.Products.Manage);
        products.MapPost(ApiRoutes.ProductById + ApiRoutes.Stock, RecordStockMovementAsync).RequirePermission(Permissions.Products.Manage);
        products.MapGet(ApiRoutes.ProductById + ApiRoutes.StockMovements, ListStockMovementsAsync).RequirePermission(Permissions.Products.View);

        return endpoints;
    }

    private static async Task<IResult> ListProductsAsync(
        bool? includeInactive,
        IUseCase<ListProductsQuery, IReadOnlyList<ProductResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListProductsQuery(includeInactive ?? false), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateProductAsync(
        CreateProductCommand command,
        IUseCase<CreateProductCommand, ProductResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(product => TypedResults.Created($"{ApiRoutes.Products}/{product.Id}", product));
    }

    private static async Task<IResult> UpdateProductAsync(
        Guid productId,
        UpdateProductRequest request,
        IUseCase<UpdateProductCommand, ProductResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(productId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SetProductActiveAsync(
        Guid productId,
        SetActiveRequest request,
        IUseCase<SetProductActiveCommand, ProductResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetProductActiveCommand(productId, request.IsActive), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RecordStockMovementAsync(
        Guid productId,
        RecordStockMovementRequest request,
        IUseCase<RecordStockMovementCommand, ProductResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(productId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListStockMovementsAsync(
        Guid productId,
        IUseCase<ListStockMovementsQuery, IReadOnlyList<StockMovementResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListStockMovementsQuery(productId), cancellationToken);

        return result.ToOkResult();
    }
}
