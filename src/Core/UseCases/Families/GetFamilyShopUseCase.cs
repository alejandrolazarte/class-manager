using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Images;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.Families;

public sealed record GetFamilyShopQuery;

public sealed class GetFamilyShopUseCase(
    IFamilyAccess familyAccess,
    IBusinessRepository businessRepository,
    IClassPackRepository classPackRepository,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage)
    : IUseCase<GetFamilyShopQuery, FamilyShopResponse>
{
    public async Task<Result<FamilyShopResponse>> ExecuteAsync(GetFamilyShopQuery command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (access is null || business is null)
        {
            return FamilyFailures.NoAccess();
        }

        var packs = await classPackRepository.ListAsync(includeInactive: false, cancellationToken);
        var products = (await productRepository.ListAsync(includeInactive: false, cancellationToken))
            .Where(product => product.IsVisibleInApp)
            .ToList();
        var stockByVariant = await stockMovementRepository.StockByVariantAsync(
            [.. products.SelectMany(product => product.Variants).Select(variant => variant.Id)], cancellationToken);

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var deliveryClasses = await DeliveryClasses.ListAsync(
            access.ClientId, today, studentRepository, enrollmentRepository, classGroupRepository, cancellationToken);

        return new FamilyShopResponse(
            business.CurrencyCode,
            [.. packs.Select(pack => new FamilyShopPackResponse(pack.Id, pack.Name, pack.ClassCount, pack.Price, pack.ValidityMonths, CatalogImageResponse.ListFrom(pack.ImagesInOrder, documentStorage)))],
            [
                .. products.Select(product => new FamilyShopProductResponse(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    [
                        .. product.Variants
                            .Where(variant => variant.IsActive)
                            .OrderBy(variant => variant.Position)
                            .Select(variant => new FamilyShopVariantResponse(
                                variant.Id,
                                variant.Name,
                                StockRules.AvailabilityOf(product, stockByVariant.GetValueOrDefault(variant.Id)))),
                    ],
                    CatalogImageResponse.ListFrom(product.ImagesInOrder, documentStorage))),
            ],
            deliveryClasses);
    }
}
