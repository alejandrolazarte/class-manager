using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_UpdateInstructor_keeps_its_own_name;

public sealed class Then_it_is_saved
{
    [Fact]
    public async Task Then_it_is_saved_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        builder.Instructors
            .Setup(repository => repository.FindByNameAsync(TestData.InstructorFullName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(builder.Instructor);
        var useCase = builder.BuildUpdate();

        var response = await useCase.ExecuteAsync(new UpdateInstructorCommand(builder.Instructor.Id, TestData.InstructorFullName), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
