using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Fees;

internal sealed class FeeUseCaseBuilder
{
    public const decimal DefaultFee = 12000m;

    public static readonly BillingMonth SinceAlways = BillingMonth.Parse("2000-01").Value!;

    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IPaymentRepository> Payments { get; } = new();
    public Mock<IFeeScheduleRepository> FeeSchedule { get; } = new();
    public Mock<IClassBalanceService> ClassBalances { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Mock<ICurrentMember> CurrentMember { get; } = new();
    public Mock<IAccessScopes> AccessScopes { get; } = new();
    public Guid CurrentUserId { get; } = Guid.CreateVersion7();
    public ClientScope CollectorScope { get; }
    public Business Business { get; } = TestData.Business();
    public BillingMonth CurrentMonth { get; } = BillingMonth.From(TestData.Today);
    public List<EnrolledStudentInPeriod> EnrolledStudents { get; } = [];
    public Dictionary<Guid, decimal> PaidByClient { get; } = [];
    public List<DefaultMonthlyFeeChange> DefaultFeeChanges { get; } = [];
    public List<ClientBillingPlanChange> PlanChanges { get; } = [];
    public Dictionary<Guid, ClassBalance> ClassBalancesByClient { get; } = [];

    public FeeUseCaseBuilder()
    {
        CollectorScope = new ClientScope(null, CurrentUserId);
        ActAs(SystemRolePermissions.BrandOwner);
        AccessScopes.Setup(scopes => scopes.ForClientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((ClientScope?)null);
        Clients.Setup(repository => repository.IsInScopeAsync(It.IsAny<Guid>(), It.IsAny<ClientScope>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        AddDefaultFeeChange(SinceAlways, DefaultFee);
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Business);
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        Enrollments
            .Setup(repository => repository.ListEnrolledInPeriodAsync(CurrentMonth.FirstDay, CurrentMonth.LastDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EnrolledStudents);
        Payments
            .Setup(repository => repository.SumByClientForMonthAsync(CurrentMonth.FirstDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaidByClient);
        FeeSchedule.Setup(repository => repository.ListDefaultFeeChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(DefaultFeeChanges);
        FeeSchedule
            .Setup(repository => repository.ListClientPlanChangesAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanChanges);
        ClassBalances
            .Setup(service => service.CalculateAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClassBalancesByClient);
    }

    public void ActAs(IReadOnlySet<string> permissions) =>
        CurrentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(CurrentUserId, Business.Id, BusinessRole.Custom, null, IsBrandOwner: false, permissions));

    public void ActAsCollector(params Guid[] clientIdsInScope)
    {
        ActAs(new HashSet<string> { Permissions.Payments.ViewOwn, Permissions.Payments.Record });
        AccessScopes.Setup(scopes => scopes.ForClientsAsync(Permissions.Payments.ViewAll, It.IsAny<CancellationToken>())).ReturnsAsync(CollectorScope);
        Clients.Setup(repository => repository.ListIdsInScopeAsync(CollectorScope, It.IsAny<CancellationToken>())).ReturnsAsync(clientIdsInScope);
        foreach (var clientId in clientIdsInScope)
        {
            Clients.Setup(repository => repository.IsInScopeAsync(clientId, CollectorScope, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        }
    }

    public void AddDefaultFeeChange(BillingMonth effectiveFrom, decimal? amount) =>
        DefaultFeeChanges.Add(DefaultMonthlyFeeChange.Create(effectiveFrom, amount, TestData.Today, TestData.Now).Value!);

    public void SetPlan(Guid clientId, BillingPlanKind kind, decimal? customFee = null, BillingMonth? effectiveFrom = null) =>
        PlanChanges.Add(ClientBillingPlanChange.Create(clientId, effectiveFrom ?? SinceAlways, kind, customFee, TestData.Today, TestData.Now).Value!);

    public Guid AddEnrolledClient(string clientFullName, decimal? clientFee = null, params string[] studentNames)
    {
        var clientId = Guid.CreateVersion7();
        foreach (var studentName in studentNames.DefaultIfEmpty(clientFullName))
        {
            EnrolledStudents.Add(new EnrolledStudentInPeriod(clientId, clientFullName, TestData.NormalizedClientPhoneNumber, studentName));
        }

        if (clientFee is not null)
        {
            SetPlan(clientId, BillingPlanKind.CustomFee, clientFee);
        }

        return clientId;
    }

    public ListMonthlyFeesUseCase BuildList() =>
        new(
            Enrollments.Object,
            Payments.Object,
            FeeSchedule.Object,
            ClassBalances.Object,
            Clients.Object,
            BusinessCalendar.Object,
            AccessScopes.Object);

    public RecordPaymentUseCase BuildRecord() =>
        new(Clients.Object, Payments.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now), AccessScopes.Object, CurrentMember.Object);

    public DeletePaymentUseCase BuildDelete() => new(Payments.Object, Clients.Object, UnitOfWork.Object, AccessScopes.Object, CurrentMember.Object);

    public SetDefaultMonthlyFeeUseCase BuildSetDefaultFee() =>
        new(Businesses.Object, FeeSchedule.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));

    public SetClientBillingPlanUseCase BuildSetClientPlan() =>
        new(Clients.Object, FeeSchedule.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now), AccessScopes.Object);

    public DeleteDefaultMonthlyFeeChangeUseCase BuildDeleteDefaultFeeChange() =>
        new(Businesses.Object, FeeSchedule.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));

    public DeleteClientBillingPlanChangeUseCase BuildDeleteClientPlanChange() =>
        new(Clients.Object, FeeSchedule.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now), AccessScopes.Object);
}
