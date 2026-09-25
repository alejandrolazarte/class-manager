using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_CreateClassGroup_with_valid_data;

public sealed class Then_class_group_is_added_and_saved
{
    [Fact]
    public async Task Then_class_group_is_added_and_saved_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();

        var response = await builder.BuildCreate().ExecuteAsync(new CreateClassGroupCommand(builder.ValidDetails()), CancellationToken.None);

        response.Value!.EndTime.ShouldBe("18:45");
        response.Value.InstructorFullName.ShouldBe(TestData.InstructorFullName);
        builder.ClassGroups.Verify(repository => repository.Add(It.IsAny<ClassGroup>()), Times.Once);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
