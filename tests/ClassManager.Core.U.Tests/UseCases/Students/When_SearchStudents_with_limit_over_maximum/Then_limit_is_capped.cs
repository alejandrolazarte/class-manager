using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_SearchStudents_with_limit_over_maximum;

public sealed class Then_limit_is_capped
{
    [Fact]
    public async Task Then_limit_is_capped_Run()
    {
        var businesses = new Mock<IBusinessRepository>();
        businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        var students = new Mock<IStudentRepository>();
        students.Setup(repository => repository.SearchAsync(It.IsAny<StudentSearchCriteria>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var useCase = new SearchStudentsUseCase(businesses.Object, students.Object, new EveryAccessScopes());

        await useCase.ExecuteAsync(new SearchStudentsQuery("tomi", 500), CancellationToken.None);

        students.Verify(repository => repository.SearchAsync(
            It.Is<StudentSearchCriteria>(criteria => criteria.Limit == SearchStudentsUseCase.MaximumLimit),
            It.IsAny<CancellationToken>()));
    }
}
