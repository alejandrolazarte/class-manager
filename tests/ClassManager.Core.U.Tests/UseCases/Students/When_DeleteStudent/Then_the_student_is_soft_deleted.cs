namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent;

public sealed class Then_the_student_is_soft_deleted
{
    [Fact]
    public async Task Then_the_student_is_soft_deleted_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();

        await builder.Build().ExecuteAsync(builder.Command(), CancellationToken.None);

        builder.Student.IsDeleted.ShouldBeTrue();
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
