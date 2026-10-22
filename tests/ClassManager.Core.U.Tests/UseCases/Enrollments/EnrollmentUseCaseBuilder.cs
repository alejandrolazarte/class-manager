using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Enrollments;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Enrollments;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments;

internal sealed class EnrollmentUseCaseBuilder
{
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IClassBalanceService> ClassBalances { get; } = new();
    public Mock<IClassPackRepository> ClassPacks { get; } = new();
    public Mock<IFeeScheduleRepository> FeeSchedule { get; } = new();
    public List<ClientBillingPlanChange> PlanChanges { get; } = [];
    public List<ClassPackUsage> ClientPackUsages { get; } = [];
    public ClassGroup ClassGroup { get; } = ClassGroup.Create(
        "Natación inicial", TestIds.Next(), ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 45).Value!, 2, null).Value!.WithId();
    public StudentSummary Student { get; } = new(
        TestIds.Next(), TestData.StudentFullName, null, null, TestIds.Next(), TestData.ClientFullName, TestData.NormalizedClientPhoneNumber);

    public EnrollmentUseCaseBuilder()
    {
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        ClassGroups.Setup(repository => repository.GetByIdAsync(ClassGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ClassGroup);
        Students.Setup(repository => repository.GetSummaryByIdAsync(Student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Student);
        ClassBalances
            .Setup(service => service.CalculateAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Dictionary<long, ClassBalance> { [Student.ClientId] = new(ClientPackUsages, []) });
        FeeSchedule
            .Setup(repository => repository.ListClientPlanChangesAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanChanges);
        FeeSchedule.Setup(repository => repository.ListDefaultFeeChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<DefaultMonthlyFeeChange>());
    }

    public void PayPerClass() =>
        PlanChanges.Add(ClientBillingPlanChange.Create(
            Student.ClientId, BillingMonth.From(TestData.Today), BillingPlanKind.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId());

    public void BuyActivePack(IReadOnlyCollection<long> classGroupIds)
    {
        var pack = ClassPack.Create("8 clases", 8, 160m, null, TestData.Now).Value!.WithId();
        pack.DefineLessons(classGroupIds, classGroupIds.Count == 0 ? 45 : null);
        ClassPacks.Setup(repository => repository.GetByIdAsync(pack.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pack);
        var purchase = ClassPackPurchase.Sell(
            Student.ClientId, pack, null, TestData.CurrencyCode, TestData.Today, PaymentMethod.Cash, null, TestData.Today, TestData.Now).Value!.WithId();
        ClientPackUsages.Add(new ClassPackUsage(purchase, 0, ClassPackPurchaseStatus.Active));
    }

    public EnrollStudentCommand ValidCommand() => new(ClassGroup.Id, Student.Id, null);

    public EnrollStudentUseCase BuildEnroll() =>
        new(
            ClassGroups.Object,
            Students.Object,
            Enrollments.Object,
            UnitOfWork.Object,
            BusinessCalendar.Object,
            new FakeTimeProvider(TestData.Now),
            new EveryAccessScopes(),
            Clients.Object,
            ClassBalances.Object,
            ClassPacks.Object,
            FeeSchedule.Object);

    public EndEnrollmentUseCase BuildEnd() =>
        new(Enrollments.Object, ClassGroups.Object, UnitOfWork.Object, BusinessCalendar.Object, new EveryAccessScopes());

    public Enrollment ExistingEnrollment(DateOnly startDate) => Enrollment.Create(Student.Id, ClassGroup.Id, startDate, TestData.Now).WithId();
}
