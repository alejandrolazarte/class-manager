using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;
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
    public ClassGroup ClassGroup { get; } = ClassGroup.Create(
        "Natación inicial", Guid.CreateVersion7(), ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 45).Value!, 2, null).Value!;
    public StudentSummary Student { get; } = new(
        Guid.CreateVersion7(), TestData.StudentFullName, null, null, Guid.CreateVersion7(), TestData.ClientFullName, TestData.NormalizedClientPhoneNumber);

    public EnrollmentUseCaseBuilder()
    {
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        ClassGroups.Setup(repository => repository.GetByIdAsync(ClassGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ClassGroup);
        Students.Setup(repository => repository.GetSummaryByIdAsync(Student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Student);
    }

    public EnrollStudentCommand ValidCommand() => new(ClassGroup.Id, Student.Id, null);

    public EnrollStudentUseCase BuildEnroll() =>
        new(ClassGroups.Object, Students.Object, Enrollments.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));

    public EndEnrollmentUseCase BuildEnd() => new(Enrollments.Object, UnitOfWork.Object, BusinessCalendar.Object);

    public Enrollment ExistingEnrollment(DateOnly startDate) => Enrollment.Create(Student.Id, ClassGroup.Id, startDate, TestData.Now);
}
