using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_without_start_date;

public sealed class Then_starts_today_in_business_time_zone
{
    [Fact]
    public async Task Then_starts_today_in_business_time_zone_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value!.StartDate.ShouldBe(TestData.Today);
        builder.Enrollments.Verify(repository => repository.Add(It.IsAny<Enrollment>()), Times.Once);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
