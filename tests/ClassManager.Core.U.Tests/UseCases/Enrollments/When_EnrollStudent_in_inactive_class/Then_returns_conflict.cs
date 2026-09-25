using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_in_inactive_class;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.ClassGroup.Deactivate();

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.Inactive);
    }
}
