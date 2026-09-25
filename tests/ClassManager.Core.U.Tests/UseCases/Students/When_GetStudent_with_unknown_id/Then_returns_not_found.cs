using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_GetStudent_with_unknown_id;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var students = new Mock<IStudentRepository>();
        var useCase = new GetStudentUseCase(students.Object);

        var response = await useCase.ExecuteAsync(new GetStudentQuery(Guid.CreateVersion7()), CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.NotFound);
    }
}
