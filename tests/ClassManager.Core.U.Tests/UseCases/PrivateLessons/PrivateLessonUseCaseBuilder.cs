using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.PrivateLessons;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons;

internal sealed class PrivateLessonUseCaseBuilder
{
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IClassSessionRepository> Sessions { get; } = new();
    public Mock<IPrivateLessonRepository> PrivateLessons { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName).Value!;
    public Instructor OtherInstructor { get; } = Instructor.Create("Lucía Díaz").Value!;
    public Guid StudentId { get; } = Guid.CreateVersion7();
    public List<PrivateLesson> AddedLessons { get; } = [];

    public PrivateLessonUseCaseBuilder()
    {
        Instructors.Setup(repository => repository.GetByIdAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
        Instructors.Setup(repository => repository.GetByIdAsync(OtherInstructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(OtherInstructor);
        Students
            .Setup(repository => repository.ListSummariesByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new StudentSummary(StudentId, TestData.StudentFullName, null, null, Guid.CreateVersion7(), TestData.ClientFullName, TestData.NormalizedClientPhoneNumber)]);
        ClassGroups.Setup(repository => repository.ListActiveByInstructorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Sessions
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Sessions
            .Setup(repository => repository.ListSubstitutionsAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        PrivateLessons
            .Setup(repository => repository.ListByInstructorOnDatesAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<DateOnly>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        PrivateLessons.Setup(repository => repository.Add(It.IsAny<PrivateLesson>())).Callback<PrivateLesson>(AddedLessons.Add);
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
    }

    public ClassGroup InstructorTeachesGroupAt(Guid instructorId, string startTime)
    {
        var classGroup = ClassGroup.Create(
            "Natación inicial",
            instructorId,
            ClassSchedule.Create([TestData.Today.DayOfWeek], startTime, 45).Value!,
            8,
            null).Value!;
        ClassGroups
            .Setup(repository => repository.ListActiveByInstructorAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([classGroup]);
        ClassGroups.Setup(repository => repository.GetByIdAsync(classGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(classGroup);
        return classGroup;
    }

    public ClassSession SubstituteTodayIn(ClassGroup classGroup, Guid substituteInstructorId)
    {
        var session = ClassSession.Create(classGroup.Id, TestData.Today, TestData.Now);
        session.AssignSubstitute(substituteInstructorId, classGroup.InstructorId);
        Sessions
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([session]);
        Sessions
            .Setup(repository => repository.ListSubstitutionsAsync(substituteInstructorId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([session]);
        return session;
    }

    public PrivateLesson ExistingLesson(DateOnly date, string startTime = "18:00") =>
        PrivateLesson.Create(
            Instructor.Id,
            date,
            ClassSchedule.Create([date.DayOfWeek], startTime, 45).Value!,
            [StudentId],
            null,
            null,
            null,
            TestData.Now).Value!;

    public SchedulePrivateLessonCommand ScheduleCommand(Guid instructorId, string startTime = "18:00", int repeatWeeks = 1) =>
        new(instructorId, [StudentId], TestData.Today, startTime, 45, "Piscina Alboraya", null, repeatWeeks);

    public SchedulePrivateLessonUseCase BuildSchedule() =>
        new(
            Instructors.Object,
            Students.Object,
            ClassGroups.Object,
            Sessions.Object,
            PrivateLessons.Object,
            UnitOfWork.Object,
            BusinessCalendar.Object,
            new FakeTimeProvider(TestData.Now),
            new Mock<IClientRepository>().Object,
            new EveryAccessScopes());

    public RecordPrivateLessonAttendanceUseCase BuildRecordAttendance() =>
        new(PrivateLessons.Object, UnitOfWork.Object, BusinessCalendar.Object, new EveryAccessScopes());
}
