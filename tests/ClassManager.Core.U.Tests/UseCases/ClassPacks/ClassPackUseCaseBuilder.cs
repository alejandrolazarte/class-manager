using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Services;
using ClassManager.Core.UseCases.ClassPacks;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks;

internal sealed class ClassPackUseCaseBuilder
{
    public Mock<IClassPackRepository> ClassPacks { get; } = new();
    public Mock<IClassPackPurchaseRepository> Purchases { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IAttendanceRepository> Attendances { get; } = new();
    public Mock<IPrivateLessonRepository> PrivateLessons { get; } = new();
    public Mock<IFeeScheduleRepository> FeeSchedule { get; } = new();
    public Mock<IOrderNumbers> OrderNumbers { get; } = new();
    public Mock<IOrderRepository> Orders { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public ClassPack Pack { get; } = ClassPack.Create("8 clases", 8, 160m, 2, TestData.Now).Value!;
    public List<ClassPackPurchase> ClientPurchases { get; } = [];
    public List<ClientAttendedClass> ClientAttendedClasses { get; } = [];
    public List<ClientAttendedClass> ClientAttendedPrivateLessons { get; } = [];
    public List<ClientBillingPlanChange> PlanChanges { get; } = [];

    public ClassPackUseCaseBuilder()
    {
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        UnitOfWork
            .Setup(unitOfWork => unitOfWork.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IUnitOfWorkTransaction>().Object);
        OrderNumbers.Setup(numbers => numbers.TakeNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        ClassPacks.Setup(repository => repository.GetByIdAsync(Pack.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Pack);
        Purchases
            .Setup(repository => repository.ListByClientsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientPurchases);
        Attendances
            .Setup(repository => repository.ListAttendedClassesByClientsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientAttendedClasses);
        PrivateLessons
            .Setup(repository => repository.ListAttendedClassesByClientsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientAttendedPrivateLessons);
        FeeSchedule
            .Setup(repository => repository.ListClientPlanChangesAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanChanges);
    }

    public void PayPerClassFrom(Guid clientId, BillingMonth effectiveFrom) =>
        PlanChanges.Add(ClientBillingPlanChange.Create(clientId, effectiveFrom, BillingPlanKind.ClassPacks, null, TestData.Today, TestData.Now).Value!);

    public void Attend(Guid clientId, DateOnly date) =>
        ClientAttendedClasses.Add(new ClientAttendedClass(clientId, new AttendedClass(date, TestData.StudentFullName, "Natación")));

    public void AttendPrivateLesson(Guid clientId, DateOnly date) =>
        ClientAttendedPrivateLessons.Add(
            new ClientAttendedClass(clientId, new AttendedClass(date, TestData.StudentFullName, TestData.InstructorFullName, IsPrivateLesson: true)));

    public ClassBalanceService BuildBalanceService() =>
        new(Purchases.Object, Attendances.Object, PrivateLessons.Object, FeeSchedule.Object, BusinessCalendar.Object);

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
            new FakeTimeProvider(TestData.Now),
            new EveryAccessScopes(),
            new Mock<ICurrentMember>().Object);

    public CreateClassPackUseCase BuildCreate() =>
        new(ClassPacks.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));
}
