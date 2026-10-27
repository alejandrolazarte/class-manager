using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_with_unknown_id;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(TestIds.Next()), CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.NotFound);
    }
}
