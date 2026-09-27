using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Sessions;

internal sealed class SessionUseCaseBuilder
{
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IClassSessionRepository> Sessions { get; } = new();
    public Mock<IAttendanceRepository> Attendances { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName).Value!;
    public ClassGroup ClassGroup { get; }
    public Guid EnrolledStudentId { get; } = Guid.CreateVersion7();

    public SessionUseCaseBuilder()
    {
        ClassGroup = ClassGroup.Create(
            "Natación inicial", Instructor.Id, ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!, 8, null).Value!;
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        ClassGroups.Setup(repository => repository.ListActiveByInstructorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Sessions.Setup(repository => repository.ListByDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        ClassGroups.Setup(repository => repository.GetByIdAsync(ClassGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ClassGroup);
        Enrollments
            .Setup(repository => repository.ListRosterOnAsync(ClassGroup.Id, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new RosterEntry(Guid.CreateVersion7(), EnrolledStudentId, TestData.StudentFullName, null, Guid.CreateVersion7(), TestData.ClientFullName, TestData.Today, null),
            ]);
    }

    public ClassSession CancelledSession()
    {
        var session = ClassSession.Create(ClassGroup.Id, TestData.Today, TestData.Now);
        session.Cancel("Feriado");
        Sessions.Setup(repository => repository.FindForUpdateAsync(ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(session);
        return session;
    }

    public RecordAttendanceUseCase BuildRecord() =>
        new(ClassGroups.Object, Enrollments.Object, Sessions.Object, Attendances.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));

    public CancelSessionUseCase BuildCancel() =>
        new(ClassGroups.Object, Sessions.Object, Attendances.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

    public RescheduleSessionUseCase BuildReschedule() =>
        new(ClassGroups.Object, Sessions.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));

    public ListMonthCalendarUseCase BuildMonthCalendar() =>
        new(ClassGroups.Object, Enrollments.Object, Sessions.Object, Attendances.Object, BusinessCalendar.Object);

    public void SetupMonthCalendar(
        IReadOnlyList<ClassGroupEnrollmentPeriod> enrollmentPeriods,
        IReadOnlyList<ClassSession> sessions,
        IReadOnlyDictionary<Guid, AttendanceCount> attendanceCounts)
    {
        ClassGroups.Setup(repository => repository.ListActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([ClassGroup]);
        Enrollments
            .Setup(repository => repository.ListActiveInPeriodAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollmentPeriods);
        Sessions
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);
        Attendances
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendanceCounts);
    }

    public ListDaySessionsUseCase BuildListDay() =>
        new(ClassGroups.Object, Instructors.Object, Enrollments.Object, Sessions.Object, Attendances.Object, BusinessCalendar.Object);
}
