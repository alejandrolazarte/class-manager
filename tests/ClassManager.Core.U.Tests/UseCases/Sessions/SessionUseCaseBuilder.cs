using ClassManager.Core.Abstractions.Notifications;
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
    public Mock<IMakeupBookingRepository> MakeupBookings { get; } = new();
    public Mock<IPackBookingRepository> PackBookings { get; } = new();
    public Mock<IPrivateLessonRepository> PrivateLessons { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName, TestData.Now).Value!.WithId();
    public Instructor Substitute { get; } = Instructor.Create("Laura Gómez", TestData.Now).Value!.WithId();
    public ClassGroup ClassGroup { get; }
    public long EnrolledStudentId { get; } = TestIds.Next();

    public SessionUseCaseBuilder()
    {
        ClassGroup = ClassGroup.Create(
            "Natación inicial", Instructor.Id, ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!, 8, null).Value!.WithId();
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        ClassGroups.Setup(repository => repository.ListActiveByInstructorAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Instructors.Setup(repository => repository.GetByIdAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
        Instructors.Setup(repository => repository.GetByIdAsync(Substitute.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Substitute);
        Instructors.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([Instructor, Substitute]);
        Sessions
            .Setup(repository => repository.ListSubstitutionsAsync(It.IsAny<long>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Sessions.Setup(repository => repository.ListByDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Sessions
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        PrivateLessons
            .Setup(repository => repository.ListByInstructorOnDatesAsync(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<DateOnly>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Students
            .Setup(repository => repository.ListSummariesByIdsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        PrivateLessons
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        ClassGroups.Setup(repository => repository.GetByIdAsync(ClassGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ClassGroup);
        MakeupBookings
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<long, int>());
        PackBookings
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<long, int>());
        PackBookings
            .Setup(repository => repository.ListStudentsBySessionAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        MakeupBookings
            .Setup(repository => repository.ListStudentsBySessionAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Enrollments
            .Setup(repository => repository.ListRosterOnAsync(ClassGroup.Id, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new RosterEntry(TestIds.Next(), EnrolledStudentId, TestData.StudentFullName, null, TestIds.Next(), TestData.ClientFullName, TestData.Today, null),
            ]);
    }

    public long EnrollAnotherStudent()
    {
        var anotherStudentId = TestIds.Next();
        Enrollments
            .Setup(repository => repository.ListRosterOnAsync(ClassGroup.Id, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new RosterEntry(TestIds.Next(), EnrolledStudentId, TestData.StudentFullName, null, TestIds.Next(), TestData.ClientFullName, TestData.Today, null),
                new RosterEntry(TestIds.Next(), anotherStudentId, TestData.StudentFullName, null, TestIds.Next(), TestData.ClientFullName, TestData.Today, null),
            ]);
        return anotherStudentId;
    }

    public ClassSession CancelledSession()
    {
        var session = ClassSession.Create(ClassGroup.Id, TestData.Today, TestData.Now).WithId();
        session.Cancel("Feriado", TestData.Now);
        Sessions.Setup(repository => repository.FindForUpdateAsync(ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(session);
        return session;
    }

    public RecordAttendanceUseCase BuildRecord() =>
        new(ClassGroups.Object, Enrollments.Object, MakeupBookings.Object, PackBookings.Object, Sessions.Object, Attendances.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now), new EveryAccessScopes());

    public RecordClassAttendanceUseCase BuildRecordClass() =>
        new(ClassGroups.Object, Enrollments.Object, MakeupBookings.Object, PackBookings.Object, Sessions.Object, Attendances.Object, UnitOfWork.Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now), new EveryAccessScopes());

    public CancelSessionUseCase BuildCancel() =>
        new(ClassGroups.Object, Sessions.Object, Attendances.Object, UnitOfWork.Object, new Mock<IStudentAppNotificationService>().Object, new FakeTimeProvider(TestData.Now));

    public RescheduleSessionUseCase BuildReschedule() =>
        new(ClassGroups.Object, Sessions.Object, PrivateLessons.Object, UnitOfWork.Object, new Mock<IStudentAppNotificationService>().Object, BusinessCalendar.Object, new FakeTimeProvider(TestData.Now));

    public AssignSubstituteUseCase BuildAssignSubstitute() =>
        new(ClassGroups.Object, Instructors.Object, Sessions.Object, PrivateLessons.Object, UnitOfWork.Object, new Mock<IStudentAppNotificationService>().Object, new FakeTimeProvider(TestData.Now));

    public ListMonthCalendarUseCase BuildMonthCalendar() =>
        new(ClassGroups.Object, Enrollments.Object, Sessions.Object, Attendances.Object, PrivateLessons.Object, BusinessCalendar.Object, new EveryAccessScopes());

    public void SetupMonthCalendar(
        IReadOnlyList<ClassGroupEnrollmentPeriod> enrollmentPeriods,
        IReadOnlyList<ClassSession> sessions,
        IReadOnlyDictionary<long, AttendanceCount> attendanceCounts)
    {
        ClassGroups.Setup(repository => repository.ListActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([ClassGroup]);
        Enrollments
            .Setup(repository => repository.ListActiveInPeriodAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollmentPeriods);
        Sessions
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);
        Attendances
            .Setup(repository => repository.CountBySessionsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendanceCounts);
    }

    public ListDaySessionsUseCase BuildListDay() =>
        new(ClassGroups.Object, Instructors.Object, Enrollments.Object, Sessions.Object, Attendances.Object, MakeupBookings.Object, PackBookings.Object, PrivateLessons.Object, Students.Object, BusinessCalendar.Object, new EveryAccessScopes());
}
