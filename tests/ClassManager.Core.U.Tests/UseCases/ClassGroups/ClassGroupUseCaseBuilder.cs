using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups;

internal sealed class ClassGroupUseCaseBuilder
{
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName).Value!;

    public ClassGroupUseCaseBuilder()
    {
        Instructors.Setup(repository => repository.GetByIdAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
        ClassGroups.Setup(repository => repository.ListActiveByInstructorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
    }

    public ClassGroupDetails ValidDetails() =>
        new("Natación inicial", Instructor.Id, [DayOfWeek.Tuesday, DayOfWeek.Thursday], "18:00", 45, 8, "Pileta chica");

    public ClassGroup ExistingClassGroup(string startTime = "18:00") =>
        ClassGroup.Create("Aquagym", Instructor.Id, ClassSchedule.Create([DayOfWeek.Thursday], startTime, 60).Value!, 10, null).Value!;

    public CreateClassGroupUseCase BuildCreate() => new(Instructors.Object, ClassGroups.Object, UnitOfWork.Object);

    public UpdateClassGroupUseCase BuildUpdate() => new(Instructors.Object, ClassGroups.Object, UnitOfWork.Object);
}
