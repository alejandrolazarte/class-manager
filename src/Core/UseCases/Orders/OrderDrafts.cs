using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.UseCases.Orders;

internal sealed record RequestedOrderLine(Guid? ClassPackId, Guid? ProductVariantId, int? Quantity, decimal? UnitPrice);

internal sealed record OrderDraft(IReadOnlyList<OrderLine> Lines, IReadOnlyDictionary<Guid, ClassPack> Packs, IReadOnlyList<Product> Products)
{
    public Product ProductOf(OrderLine line) => Products.First(product => product.Id == line.ProductId);
}

internal enum OrderAudience
{
    Team,
    Family,
}

internal static class OrderDrafts
{
    public static async Task<Result<OrderDraft>> BuildAsync(
        IReadOnlyList<RequestedOrderLine> requestedLines,
        OrderAudience audience,
        IClassPackRepository classPackRepository,
        IProductRepository productRepository,
        CancellationToken cancellationToken)
    {
        var packs = await LoadPacksAsync(requestedLines.Select(line => line.ClassPackId), classPackRepository, cancellationToken);
        var products = await productRepository.ListByVariantsAsync(
            [.. requestedLines.Select(line => line.ProductVariantId).OfType<Guid>().Distinct()], cancellationToken);

        var lines = new List<OrderLine>();
        foreach (var requestedLine in requestedLines)
        {
            var line = BuildLine(requestedLine, audience, packs, products);
            if (line.IsFailure)
            {
                return line.Error!;
            }

            lines.Add(line.Value!);
        }

        return new OrderDraft(lines, packs, products);
    }

    public static async Task<Dictionary<Guid, ClassPack>> LoadPacksAsync(
        IEnumerable<Guid?> classPackIds,
        IClassPackRepository classPackRepository,
        CancellationToken cancellationToken)
    {
        var packs = new Dictionary<Guid, ClassPack>();
        foreach (var classPackId in classPackIds.OfType<Guid>().Distinct())
        {
            if (await classPackRepository.GetByIdAsync(classPackId, cancellationToken) is { } pack)
            {
                packs[classPackId] = pack;
            }
        }

        return packs;
    }

    public static async Task<ResultError?> LockAndCheckStockAsync(
        OrderDraft draft,
        IStockLock stockLock,
        IStockMovementRepository stockMovementRepository,
        CancellationToken cancellationToken)
    {
        var quantityByVariant = draft.Lines
            .Where(line => line.Kind == OrderLineKind.Product && draft.ProductOf(line).TracksStock)
            .GroupBy(line => line.ProductVariantId!.Value)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));
        if (quantityByVariant.Count == 0)
        {
            return null;
        }

        await stockLock.LockAsync([.. quantityByVariant.Keys], cancellationToken);
        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. quantityByVariant.Keys], cancellationToken);
        foreach (var (variantId, quantity) in quantityByVariant)
        {
            var product = draft.Products.First(candidate => candidate.FindVariant(variantId) is not null);
            if (!StockRules.CanSell(product, stockByVariant.GetValueOrDefault(variantId), quantity))
            {
                return ProductFailures.OutOfStock(product.SaleNameOf(product.FindVariant(variantId)!));
            }
        }

        return null;
    }

    private static Result<OrderLine> BuildLine(
        RequestedOrderLine requestedLine,
        OrderAudience audience,
        Dictionary<Guid, ClassPack> packs,
        IReadOnlyList<Product> products)
    {
        var unitPrice = audience == OrderAudience.Team ? requestedLine.UnitPrice : null;
        switch (requestedLine)
        {
            case { ClassPackId: { } classPackId, ProductVariantId: null }:
                if (!packs.TryGetValue(classPackId, out var pack))
                {
                    return ClassPackFailures.NotFound();
                }

                return pack.IsActive ? OrderLine.ForClassPack(pack, unitPrice) : ProductFailures.NotSold();
            case { ProductVariantId: { } variantId, ClassPackId: null }:
                var product = products.FirstOrDefault(candidate => candidate.FindVariant(variantId) is not null);
                if (product is null || (audience == OrderAudience.Family && !product.IsVisibleInApp))
                {
                    return ProductFailures.VariantNotFound();
                }

                var variant = product.FindVariant(variantId)!;
                if (!product.IsActive || !variant.IsActive)
                {
                    return ProductFailures.NotSold();
                }

                return OrderLine.ForProduct(product, variant, requestedLine.Quantity, unitPrice);
            default:
                return OrderFailures.LineItemRequired();
        }
    }
}
