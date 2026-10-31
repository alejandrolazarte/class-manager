using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Services;
using ClassManager.Core.UseCases.Fees;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Fees;

internal sealed class FeeUseCaseBuilder
{
    public const decimal DefaultFee = 12000m;


    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IPaymentRepository> Payments { get; } = new();
    public Mock<IFamilyBillingService> FamilyBilling { get; } = new();
    public Mock<IClassBalanceService> ClassBalances { get; } = new();
    public Mock<IOrderRepository> Orders { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Mock<ICurrentMember> CurrentMember { get; } = new();
    public Mock<IAccessScopes> AccessScopes { get; } = new();
    public Guid CurrentAccountId { get; } = Guid.CreateVersion7();
    public long CurrentUserId { get; } = TestIds.Next();
    public ClientScope CollectorScope { get; }
    public Business Business { get; } = TestData.Business();
    public BillingMonth CurrentMonth { get; } = BillingMonth.From(TestData.Today);
    public List<EnrolledStudentInPeriod> EnrolledStudents { get; } = [];
    public Dictionary<long, decimal> PaidByClient { get; } = [];
    public Dictionary<long, FamilyMonthBilling> BillingByClient { get; } = [];
    public Dictionary<long, ClassBalance> ClassBalancesByClient { get; } = [];

    public FeeUseCaseBuilder()
    {
        CollectorScope = new ClientScope(null, CurrentUserId);
        ActAs(SystemRolePermissions.BrandOwner);
        AccessScopes.Setup(scopes => scopes.ForClientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((ClientScope?)null);
        Clients.Setup(repository => repository.IsInScopeAsync(It.IsAny<long>(), It.IsAny<ClientScope>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Business);
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        Enrollments
            .Setup(repository => repository.ListEnrolledInPeriodAsync(CurrentMonth.FirstDay, CurrentMonth.LastDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EnrolledStudents);
        Payments
            .Setup(repository => repository.SumByClientForMonthAsync(CurrentMonth.FirstDay, TestData.CurrencyCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaidByClient);
        FamilyBilling
            .Setup(service => service.CalculateAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<BillingMonth>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<long> clientIds, BillingMonth _, CancellationToken _) =>
                clientIds.Distinct().ToDictionary(clientId => clientId, clientId => BillingByClient.GetValueOrDefault(clientId) ?? new FamilyMonthBilling(clientId, [])));
        Orders
            .Setup(repository => repository.ListPaidBetweenOrOwingAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<ClientScope?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        ClassBalances
            .Setup(service => service.CalculateAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClassBalancesByClient);
    }

    public void ActAs(IReadOnlySet<string> permissions) =>
        CurrentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(CurrentAccountId, CurrentUserId, Business.Id, BusinessRole.Custom, null, IsBrandOwner: false, permissions));

    public void ActAsCollector(params long[] clientIdsInScope)
    {
        ActAs(new HashSet<string> { Permissions.Payments.ViewOwn, Permissions.Payments.Record });
        AccessScopes.Setup(scopes => scopes.ForClientsAsync(Permissions.Payments.ViewAll, It.IsAny<CancellationToken>())).ReturnsAsync(CollectorScope);
        Clients.Setup(repository => repository.ListIdsInScopeAsync(CollectorScope, It.IsAny<CancellationToken>())).ReturnsAsync(clientIdsInScope);
        foreach (var clientId in clientIdsInScope)
        {
            Clients.Setup(repository => repository.IsInScopeAsync(clientId, CollectorScope, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        }
    }

    public void SetMonthlyPrice(long clientId, decimal price) =>
        BillingByClient[clientId] = new FamilyMonthBilling(clientId, [new StudentPlanLine(TestIds.Next(), TestData.StudentFullName, TestIds.Next(), "Cuota mensual", price, null)]);

    public void PaysPerClass(long clientId) => BillingByClient[clientId] = new FamilyMonthBilling(clientId, []);

    public long AddEnrolledClient(string clientFullName, decimal? clientFee = null, params string[] studentNames)
    {
        var clientId = TestIds.Next();
        foreach (var studentName in studentNames.DefaultIfEmpty(clientFullName))
        {
            EnrolledStudents.Add(new EnrolledStudentInPeriod(clientId, clientFullName, TestData.NormalizedClientPhoneNumber, studentName));
        }

        SetMonthlyPrice(clientId, clientFee ?? DefaultFee);

        return clientId;
    }

    public ListMonthlyFeesUseCase BuildList() =>
        new(
            Enrollments.Object,
            Payments.Object,
            FamilyBilling.Object,
            ClassBalances.Object,
            Clients.Object,
            Orders.Object,
            BusinessCalendar.Object,
            new BusinessCurrencyService(Businesses.Object),
            AccessScopes.Object);

    public RecordPaymentUseCase BuildRecord() =>
        new(
            Clients.Object,
            Payments.Object,
            UnitOfWork.Object,
            BusinessCalendar.Object,
            new BusinessCurrencyService(Businesses.Object),
            new FakeTimeProvider(TestData.Now),
            AccessScopes.Object,
            CurrentMember.Object);

    public DeletePaymentUseCase BuildDelete() => new(Payments.Object, Clients.Object, UnitOfWork.Object, AccessScopes.Object, CurrentMember.Object);
}
