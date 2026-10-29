namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_has_a_pack_for_any_group_class;

public sealed class Then_the_student_is_enrolled
{
    [Fact]
    public async Task Then_the_student_is_enrolled_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.PayPerClass();
        builder.BuyActivePack([], coversAnyGroupClass: true);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value!.StudentId.ShouldBe(builder.Student.Id);
    }
}
