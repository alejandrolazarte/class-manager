namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_in_a_class_of_the_pack;

public sealed class Then_the_student_is_enrolled
{
    [Fact]
    public async Task Then_the_student_is_enrolled_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.PayPerClass();
        builder.BuyActivePack([builder.ClassGroup.Id]);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value!.StudentId.ShouldBe(builder.Student.Id);
    }
}
