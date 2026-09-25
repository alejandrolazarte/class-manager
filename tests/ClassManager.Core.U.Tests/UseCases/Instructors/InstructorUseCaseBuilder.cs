using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.Instructors;

internal sealed class InstructorUseCaseBuilder
{
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName).Value!;

    public InstructorUseCaseBuilder()
    {
        Instructors.Setup(repository => repository.GetForUpdateAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
    }
}
