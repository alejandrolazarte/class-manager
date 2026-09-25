using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Fees;
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
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Business Business { get; } = TestData.Business();
    public BillingMonth CurrentMonth { get; } = BillingMonth.From(TestData.Today);
    public List<EnrolledStudentInPeriod> EnrolledStudents { get; } = [];
    public Dictionary<Guid, decimal> PaidByClient { get; } = [];

    public FeeUseCaseBuilder()
    {
        Business.SetDefaultMonthlyFee(DefaultFee);
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Business);
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        Enrollments
            .Setup(repository => repository.ListEnrolledInPeriodAsync(CurrentMonth.FirstDay, CurrentMonth.LastDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EnrolledStudents);
        Payments
            .Setup(repository => repository.SumByClientForMonthAsync(CurrentMonth.FirstDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaidByClient);
    }

    public Guid AddEnrolledClient(string clientFullName, decimal? clientFee = null, params string[] studentNames)
    {
        var clientId = Guid.CreateVersion7();
        foreach (var studentName in studentNames.DefaultIfEmpty(clientFullName))
        {
            EnrolledStudents.Add(new EnrolledStudentInPeriod(clientId, clientFullName, TestData.NormalizedClientPhoneNumber, clientFee, studentName));
        }

        return clientId;
    }

    public ListMonthlyFeesUseCase BuildList() =>
        new(Businesses.Object, Enrollments.Object, Payments.Object, BusinessCalendar.Object);

    public RecordPaymentUseCase BuildRecord() =>
        new(Clients.Object, Payments.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));
}
