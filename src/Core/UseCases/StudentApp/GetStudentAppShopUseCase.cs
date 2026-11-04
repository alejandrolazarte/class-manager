using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Images;
using ClassManager.Core.UseCases.Orders;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppShopQuery : IQuery;

public sealed class GetStudentAppShopUseCase(
    IStudentAppFamilyService studentAppFamily,
    IBusinessRepository businessRepository,
    IClassPackRepository classPackRepository,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IStudentRepository studentRepository,
    IStudentPlanRepository studentPlanRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage,
    IFeatureAccess featureAccess)
    : IUseCase<GetStudentAppShopQuery, StudentAppShopResponse>
{
    public async Task<Result<StudentAppShopResponse>> ExecuteAsync(GetStudentAppShopQuery command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (family is null || business is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var everyClassPack = await classPackRepository.ListAsync(includeInactive: true, cancellationToken);
        var catalog = everyClassPack.Where(classPack => classPack.IsOfferedInApp).ToList();
        var planNames = everyClassPack.Where(classPack => classPack.RenewsMonthly).ToDictionary(plan => plan.Id, plan => plan.Name);
        var features = await featureAccess.GetCurrentAsync(cancellationToken);
        var products = features.Has(Features.Shop)
            ? (await productRepository.ListAsync(includeInactive: false, cancellationToken))
                .Where(product => product.IsVisibleInApp)
                .ToList()
            : [];
        var stockByVariant = await stockMovementRepository.StockByVariantAsync(
            [.. products.SelectMany(product => product.Variants).Select(variant => variant.Id)], cancellationToken);

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var currentMonth = BillingMonth.From(today);
        var priceChanges = await classPackRepository.ListPriceChangesAsync(cancellationToken);
        var planChanges = await studentPlanRepository.ListByClientsAsync([family.Client.Id], cancellationToken);
        var deliveryClasses = await DeliveryClasses.ListAsync(
            family.Client.Id, today, studentRepository, enrollmentRepository, classGroupRepository, cancellationToken);

        return new StudentAppShopResponse(
            business.CurrencyCode,
            [.. catalog.Where(pack => pack.IsSoldOnce).Select(pack => new StudentAppShopPackResponse(pack.Id, pack.Name, pack.Description, pack.ClassCount!.Value, pack.Price, pack.ValidityMonths, CatalogImageResponse.ListFrom(pack.ImagesInOrder, documentStorage)))],
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
                                variant.Size,
                                variant.Color,
                                StockRules.AvailabilityOf(product, stockByVariant.GetValueOrDefault(variant.Id)),
                                StockRules.MaximumUnitsOf(product, stockByVariant.GetValueOrDefault(variant.Id)))),
                    ],
                    CatalogImageResponse.ListFrom(product.ImagesInOrder, documentStorage))),
            ],
            deliveryClasses,
            family.CanBuy,
            [
                .. catalog
                    .Where(plan => plan.RenewsMonthly)
                    .Select(plan => new StudentAppShopPlanResponse(
                        plan.Id,
                        plan.Name,
                        plan.Description,
                        plan.ClassCount,
                        PlanPrices.PriceIn(plan, priceChanges, currentMonth),
                        CatalogImageResponse.ListFrom(plan.ImagesInOrder, documentStorage))),
            ],
            [
                .. family.Students
                    .OrderBy(student => student.User.FullName, StringComparer.CurrentCultureIgnoreCase)
                    .Select(student => PlanStudentOf(student, planChanges, planNames, currentMonth)),
            ]);
    }

    private static StudentAppPlanStudentResponse PlanStudentOf(
        Student student,
        IReadOnlyCollection<StudentPlanChange> planChanges,
        IReadOnlyDictionary<long, string> planNames,
        BillingMonth currentMonth)
    {
        var studentChanges = planChanges.Where(change => change.StudentId == student.Id).ToList();
        var currentPlanId = StudentPlanTimeline.ChangeIn(studentChanges, currentMonth)?.ClassPackId;
        return new StudentAppPlanStudentResponse(
            student.Id,
            student.User.FullName,
            currentPlanId,
            currentPlanId is { } planId ? planNames.GetValueOrDefault(planId) : null,
            StudentPlanTimeline.ChangeIn(studentChanges, currentMonth.AddMonths(1))?.ClassPackId);
    }
}
