using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Services;
using ClassManager.Core.UseCases.ClassPacks;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks;

internal sealed class ClassPackUseCaseBuilder
{
    public Mock<IClassPackRepository> ClassPacks { get; } = new();
    public Mock<IClassPackPurchaseRepository> Purchases { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IAttendanceRepository> Attendances { get; } = new();
    public Mock<IPrivateLessonRepository> PrivateLessons { get; } = new();
    public Mock<IStudentPlanRepository> StudentPlans { get; } = new();
    public Mock<IPaymentRepository> Payments { get; } = new();
    public Mock<IOrderNumbers> OrderNumbers { get; } = new();
    public Mock<IOrderRepository> Orders { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Mock<IBusinessCurrencyService> BusinessCurrency { get; } = new();
    public static readonly long ClassGroupId = TestIds.Next();

    public ClassPack Pack { get; } = AnyGroupClassPack();
    public List<ClassPackPurchase> ClientPurchases { get; } = [];
    public List<ClientAttendedClass> ClientAttendedClasses { get; } = [];
    public List<ClientAttendedClass> ClientAttendedPrivateLessons { get; } = [];
    public List<StudentPlanChange> PlanChanges { get; } = [];
    public List<ClassPack> Catalog { get; } = [];
    public Client Client { get; } = TestData.Client();
    public Student Student { get; }
    public List<ClientPaidMonth> PaidMonths { get; } = [];

    private static ClassPack AnyGroupClassPack()
    {
        var pack = ClassPack.Create("8 clases", 8, 160m, 2, TestData.Now).Value!.WithId();
        pack.DefineLessons([], null, coversAnyGroupClass: true);
        return pack;
    }

    public ClassPackUseCaseBuilder()
    {
        Student = TestUsers.StudentOf(Client, TestData.StudentFullName);
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        BusinessCurrency.Setup(currency => currency.CurrencyCodeAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.CurrencyCode);
        UnitOfWork
            .Setup(unitOfWork => unitOfWork.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IUnitOfWorkTransaction>().Object);
        OrderNumbers.Setup(numbers => numbers.TakeNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        ClassPacks.Setup(repository => repository.GetByIdAsync(Pack.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Pack);
        Purchases
            .Setup(repository => repository.ListByClientsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientPurchases);
        Attendances
            .Setup(repository => repository.ListAttendedClassesByClientsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientAttendedClasses);
        PrivateLessons
            .Setup(repository => repository.ListAttendedClassesByClientsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientAttendedPrivateLessons);
        StudentPlans
            .Setup(repository => repository.ListByClientsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanChanges);
        ClassPacks.Setup(repository => repository.ListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(Catalog);
        Payments
            .Setup(repository => repository.ListPaidMonthsByClientsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaidMonths);
    }

    public void HoldMonthlyPlanFrom(BillingMonth firstMonthWithPlan)
    {
        var monthlyPlan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();
        monthlyPlan.DefineLessons([], null, coversAnyGroupClass: true);
        Catalog.Add(monthlyPlan);
        PlanChanges.Add(StudentPlanChange.ForStudent(Student, firstMonthWithPlan, monthlyPlan, null, TestData.Today, TestData.Now).Value!);
    }

    public void HoldMonthlyPlanUntil(BillingMonth firstMonthWithoutPlan)
    {
        HoldMonthlyPlanFrom(firstMonthWithoutPlan.AddMonths(-1));
        PlanChanges.Add(StudentPlanChange.ForStudent(Student, firstMonthWithoutPlan, null, null, TestData.Today, TestData.Now).Value!);
    }

    public void PayFee(long clientId, BillingMonth month) => PaidMonths.Add(new ClientPaidMonth(clientId, month.FirstDay));

    public void Attend(long clientId, DateOnly date) =>
        ClientAttendedClasses.Add(new ClientAttendedClass(clientId, new AttendedClass(date, TestData.StudentFullName, "Natación", StudentId: Student.Id, ClassGroupId: ClassGroupId)));

    public void AttendPrivateLesson(long clientId, DateOnly date) =>
        ClientAttendedPrivateLessons.Add(
            new ClientAttendedClass(clientId, new AttendedClass(date, TestData.StudentFullName, TestData.InstructorFullName, IsPrivateLesson: true, StudentId: Student.Id)));

    public ClassBalanceService BuildBalanceService() =>
        new(Purchases.Object, Attendances.Object, PrivateLessons.Object, new MonthlyPlansReader(StudentPlans.Object, ClassPacks.Object), Payments.Object, BusinessCalendar.Object);

    public SellClassPackUseCase BuildSell() =>
        new(
            Clients.Object,
            ClassPacks.Object,
            Purchases.Object,
            PrivateLessons.Object,
            OrderNumbers.Object,
            Orders.Object,
            UnitOfWork.Object,
            BusinessCalendar.Object,
            BusinessCurrency.Object,
            new FakeTimeProvider(TestData.Now),
            new EveryAccessScopes(),
            new Mock<ICurrentMember>().Object);

    public CreateClassPackUseCase BuildCreate() =>
        new(ClassPacks.Object, ClassGroups.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now), BusinessCalendar.Object, new Mock<IDocumentStorageService>().Object);
}
