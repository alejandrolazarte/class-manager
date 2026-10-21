namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_outside_the_pack_is_confirmed;

public sealed class Then_the_student_is_enrolled
{
    [Fact]
    public async Task Then_the_student_is_enrolled_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.PayPerClass();
        builder.BuyActivePack([Guid.CreateVersion7()]);

        var response = await builder.BuildEnroll().ExecuteAsync(
            builder.ValidCommand() with { AllowOutsidePack = true }, CancellationToken.None);

        response.Value!.StudentId.ShouldBe(builder.Student.Id);
    }
}
