namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_with_valid_data;

public sealed class Then_the_new_details_are_saved
{
    [Fact]
    public async Task Then_the_new_details_are_saved_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value!.FullName.ShouldBe(UpdateStudentUseCaseBuilder.NewFullName);
        response.Value.BirthDate.ShouldBe(UpdateStudentUseCaseBuilder.NewBirthDate);
        response.Value.Notes.ShouldBe(UpdateStudentUseCaseBuilder.NewNotes);
        response.Value.Email.ShouldBe(UpdateStudentUseCaseBuilder.NewEmail);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
