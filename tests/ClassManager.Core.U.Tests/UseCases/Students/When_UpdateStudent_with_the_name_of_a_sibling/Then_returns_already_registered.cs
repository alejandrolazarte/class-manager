using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_with_the_name_of_a_sibling;

public sealed class Then_returns_already_registered
{
    [Fact]
    public async Task Then_returns_already_registered_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();
        builder.WithSibling(UpdateStudentUseCaseBuilder.NewFullName);

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.AlreadyRegistered);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
