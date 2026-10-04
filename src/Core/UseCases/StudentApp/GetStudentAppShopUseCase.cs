using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Images;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppShopQuery : IQuery;

public sealed class GetStudentAppShopUseCase(
    IStudentAppAccess studentAppAccess,
    IBusinessRepository businessRepository,
    IClassPackRepository classPackRepository,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage)
    : IUseCase<GetStudentAppShopQuery, StudentAppShopResponse>
{
    public async Task<Result<StudentAppShopResponse>> ExecuteAsync(GetStudentAppShopQuery command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (access is null || business is null)
        {
            return StudentAppFailures.NoAccess();
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

        return new StudentAppShopResponse(
            business.CurrencyCode,
            [.. packs.Select(pack => new StudentAppShopPackResponse(pack.Id, pack.Name, pack.Description, pack.ClassCount, pack.Price, pack.ValidityMonths, CatalogImageResponse.ListFrom(pack.ImagesInOrder, documentStorage)))],
            [
                .. products.Select(product => new StudentAppShopProductResponse(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    [
                        .. product.Variants
                            .Where(variant => variant.IsActive)
                            .OrderBy(variant => variant.Position)
                            .Select(variant => new StudentAppShopVariantResponse(
                                variant.Id,
                                variant.Name,
                                StockRules.AvailabilityOf(product, stockByVariant.GetValueOrDefault(variant.Id)))),
                    ],
                    CatalogImageResponse.ListFrom(product.ImagesInOrder, documentStorage))),
            ],
            deliveryClasses);
    }
}
