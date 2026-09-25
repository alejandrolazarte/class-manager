using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_in_full_class;

public sealed class Then_returns_conflict_with_capacity
{
    [Fact]
    public async Task Then_returns_conflict_with_capacity_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.Enrollments
            .Setup(repository => repository.CountCurrentAsync(builder.ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(builder.ClassGroup.Capacity);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.Full);
        response.Error.Details[ClassGroupErrorCodes.CapacityDetail].ShouldBe(builder.ClassGroup.Capacity);
    }
}
