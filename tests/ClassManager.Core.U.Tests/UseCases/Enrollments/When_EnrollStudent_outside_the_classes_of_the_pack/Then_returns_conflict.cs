using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_outside_the_classes_of_the_pack;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.PayPerClass();
        builder.BuyActivePack([TestIds.Next()]);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(EnrollmentErrorCodes.OutsidePack);
    }
}
