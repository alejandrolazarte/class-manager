using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_SetClassGroupActive_false_with_enrollments;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var classGroup = builder.ExistingClassGroup();
        builder.ClassGroups.Setup(repository => repository.GetForUpdateAsync(classGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(classGroup);
        builder.Enrollments
            .Setup(repository => repository.CountCurrentAsync(classGroup.Id, TestData.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var response = await builder.BuildSetActive().ExecuteAsync(new SetClassGroupActiveCommand(classGroup.Id, false), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.HasEnrollments);
        classGroup.IsActive.ShouldBeTrue();
    }
}
