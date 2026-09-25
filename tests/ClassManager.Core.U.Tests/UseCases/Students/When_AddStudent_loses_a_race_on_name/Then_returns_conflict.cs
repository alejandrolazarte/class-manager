using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_loses_a_race_on_name;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        builder.UnitOfWork
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UniqueConstraintViolationException());

        var response = await builder.Build().ExecuteAsync(AddStudentUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.AlreadyRegistered);
    }
}
