using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups;

internal sealed class ClassGroupUseCaseBuilder
{
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IPrivateLessonRepository> PrivateLessons { get; } = new();
    public Mock<IBusinessCalendarService> BusinessCalendar { get; } = new();
    public Mock<IDocumentRepository> Documents { get; } = new();
    public Mock<IDocumentStorageService> DocumentStorage { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName).Value!;

    public ClassGroupUseCaseBuilder()
    {
        Instructors.Setup(repository => repository.GetByIdAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
        ClassGroups.Setup(repository => repository.ListActiveByInstructorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        BusinessCalendar.Setup(calendar => calendar.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        PrivateLessons
            .Setup(repository => repository.ListByInstructorFromAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    public ClassGroupDetails ValidDetails() =>
        new("Natación inicial", Instructor.Id, [DayOfWeek.Tuesday, DayOfWeek.Thursday], "18:00", 45, 8, "Pileta chica");

    public ClassGroup ExistingClassGroup(string startTime = "18:00") =>
        ClassGroup.Create("Aquagym", Instructor.Id, ClassSchedule.Create([DayOfWeek.Thursday], startTime, 60).Value!, 10, null).Value!;

    public CreateClassGroupUseCase BuildCreate() =>
        new(Instructors.Object, ClassGroups.Object, PrivateLessons.Object, UnitOfWork.Object, BusinessCalendar.Object, DocumentStorage.Object);

    public UpdateClassGroupUseCase BuildUpdate() =>
        new(Instructors.Object, ClassGroups.Object, Enrollments.Object, PrivateLessons.Object, UnitOfWork.Object, BusinessCalendar.Object, Documents.Object, DocumentStorage.Object);

    public SetClassGroupActiveUseCase BuildSetActive() =>
        new(Instructors.Object, ClassGroups.Object, Enrollments.Object, UnitOfWork.Object, BusinessCalendar.Object, DocumentStorage.Object);
}
