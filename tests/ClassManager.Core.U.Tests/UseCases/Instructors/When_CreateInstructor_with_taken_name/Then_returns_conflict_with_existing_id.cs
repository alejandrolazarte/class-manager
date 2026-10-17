using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Instructors;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_CreateInstructor_with_taken_name;

public sealed class Then_returns_conflict_with_existing_id
{
    [Fact]
    public async Task Then_returns_conflict_with_existing_id_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        builder.Instructors
            .Setup(repository => repository.FindByNameAsync(TestData.InstructorFullName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(builder.Instructor);
        var useCase = new CreateInstructorUseCase(builder.Instructors.Object, builder.Users.Object, builder.PeopleLock.Object, builder.UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new CreateInstructorCommand(TestData.InstructorFullName), CancellationToken.None);

        response.Error!.Code.ShouldBe(InstructorErrorCodes.NameTaken);
        response.Error.Details[InstructorErrorCodes.ExistingInstructorIdDetail].ShouldBe(builder.Instructor.Id);
    }
}
