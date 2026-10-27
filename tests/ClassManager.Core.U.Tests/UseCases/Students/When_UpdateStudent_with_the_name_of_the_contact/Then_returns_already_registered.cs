using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_with_the_name_of_the_contact;

public sealed class Then_returns_already_registered
{
    [Fact]
    public async Task Then_returns_already_registered_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand() with { FullName = TestData.ClientFullName }, CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.AlreadyRegistered);
    }
}
