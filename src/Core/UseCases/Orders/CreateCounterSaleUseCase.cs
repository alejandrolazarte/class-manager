using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.UseCases.Orders;

public sealed record CounterSaleLine(Guid? ClassPackId, Guid? ProductVariantId, int? Quantity, decimal? UnitPrice);

public sealed record CreateCounterSaleCommand(
    Guid? ClientId,
    IReadOnlyList<CounterSaleLine>? Lines,
    PaymentMethod? Method,
    DateOnly? PaidOn,
    string? Notes,
    bool IsDelivered);

public sealed class CreateCounterSaleUseCase(
    IClientRepository clientRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<CreateCounterSaleCommand, OrderResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<OrderResponse>> ExecuteAsync(CreateCounterSaleCommand command, CancellationToken cancellationToken)
    {
        Client? client = null;
        if (command.ClientId is { } clientId)
        {
            client = await clientRepository.GetByIdAsync(clientId, cancellationToken);
            if (client is null)
            {
                return Result.NotFound<OrderResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
            }

            if (!await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], Permissions.Orders.ViewAll, cancellationToken))
            {
                return AccessRules.NotYours();
            }
        }

        var requestedLines = command.Lines ?? [];
        var packs = await LoadPacksAsync(requestedLines, cancellationToken);
        var products = await productRepository.ListByVariantsAsync(
            [.. requestedLines.Select(line => line.ProductVariantId).OfType<Guid>().Distinct()], cancellationToken);

        var lines = new List<OrderLine>();
        foreach (var requestedLine in requestedLines)
        {
            var line = BuildLine(requestedLine, packs, products);
            if (line.IsFailure)
            {
                return line.Error!;
            }

            lines.Add(line.Value!);
        }

        var stockCheck = await CheckStockAsync(lines, products, cancellationToken);
        if (stockCheck is not null)
        {
            return stockCheck;
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var order = Order.CounterSale(
            client?.Id, lines, command.Method, command.PaidOn ?? today, command.Notes, command.IsDelivered, today, access?.UserId, now);
        if (order.IsFailure)
        {
            return order.Error!;
        }

        foreach (var line in order.Value!.Lines)
        {
            if (line.Kind == OrderLineKind.ClassPack)
            {
                var purchase = ClassPackPurchase.Sell(
                    client!.Id, packs[line.ClassPackId!.Value], line.UnitPrice, order.Value.PaidOn!.Value, order.Value.Method, order.Value.Notes, today, now, access?.UserId);
                if (purchase.IsFailure)
                {
                    return purchase.Error!;
                }

                purchaseRepository.Add(purchase.Value!);
                line.LinkPurchase(purchase.Value!.Id);
                continue;
            }

            var product = products.First(candidate => candidate.Id == line.ProductId);
            if (product.TracksStock)
            {
                stockMovementRepository.Add(StockMovement.Sale(line.ProductVariantId!.Value, line.Quantity, order.Value.Id, access?.UserId, now));
            }
        }

        orderRepository.Add(order.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderResponse.From(order.Value, client);
    }

    private async Task<Dictionary<Guid, ClassPack>> LoadPacksAsync(IReadOnlyList<CounterSaleLine> lines, CancellationToken cancellationToken)
    {
        var packs = new Dictionary<Guid, ClassPack>();
        foreach (var classPackId in lines.Select(line => line.ClassPackId).OfType<Guid>().Distinct())
        {
            if (await classPackRepository.GetByIdAsync(classPackId, cancellationToken) is { } pack)
            {
                packs[classPackId] = pack;
            }
        }

        return packs;
    }

    private static Result<OrderLine> BuildLine(
        CounterSaleLine requestedLine,
        Dictionary<Guid, ClassPack> packs,
        IReadOnlyList<Product> products)
    {
        switch (requestedLine)
        {
            case { ClassPackId: { } classPackId, ProductVariantId: null }:
                if (!packs.TryGetValue(classPackId, out var pack))
                {
                    return ClassPackFailures.NotFound();
                }

                return pack.IsActive ? OrderLine.ForClassPack(pack, requestedLine.UnitPrice) : ProductFailures.NotSold();
            case { ProductVariantId: { } variantId, ClassPackId: null }:
                var product = products.FirstOrDefault(candidate => candidate.FindVariant(variantId) is not null);
                if (product is null)
                {
                    return ProductFailures.VariantNotFound();
                }

                var variant = product.FindVariant(variantId)!;
                if (!product.IsActive || !variant.IsActive)
                {
                    return ProductFailures.NotSold();
                }

                return OrderLine.ForProduct(product, variant, requestedLine.Quantity, requestedLine.UnitPrice);
            default:
                return OrderFailures.LineItemRequired();
        }
    }

    private async Task<ResultError?> CheckStockAsync(
        IReadOnlyList<OrderLine> lines,
        IReadOnlyList<Product> products,
        CancellationToken cancellationToken)
    {
        var quantityByVariant = lines
            .Where(line => line.Kind == OrderLineKind.Product)
            .GroupBy(line => line.ProductVariantId!.Value)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));
        if (quantityByVariant.Count == 0)
        {
            return null;
        }

        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. quantityByVariant.Keys], cancellationToken);
        foreach (var (variantId, quantity) in quantityByVariant)
        {
            var product = products.First(candidate => candidate.FindVariant(variantId) is not null);
            if (!StockRules.CanSell(product, stockByVariant.GetValueOrDefault(variantId), quantity))
            {
                return ProductFailures.OutOfStock(product.SaleNameOf(product.FindVariant(variantId)!));
            }
        }

        return null;
    }
}
