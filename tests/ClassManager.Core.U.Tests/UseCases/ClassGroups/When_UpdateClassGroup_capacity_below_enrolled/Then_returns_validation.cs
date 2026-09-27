using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_UpdateClassGroup_capacity_below_enrolled;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var classGroup = builder.ExistingClassGroup();
        builder.ClassGroups.Setup(repository => repository.GetForUpdateAsync(classGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(classGroup);
        builder.Enrollments
            .Setup(repository => repository.CountCurrentAsync(classGroup.Id, TestData.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(9);

        var response = await builder.BuildUpdate().ExecuteAsync(new UpdateClassGroupCommand(classGroup.Id, builder.ValidDetails()), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(ClassGroupDetails.Capacity));
    }
}
