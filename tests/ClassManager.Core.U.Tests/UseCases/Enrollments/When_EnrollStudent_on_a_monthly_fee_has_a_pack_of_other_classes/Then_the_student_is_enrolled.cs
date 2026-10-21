namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_on_a_monthly_fee_has_a_pack_of_other_classes;

public sealed class Then_the_student_is_enrolled
{
    [Fact]
    public async Task Then_the_student_is_enrolled_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        builder.BuyActivePack([Guid.CreateVersion7()]);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value!.StudentId.ShouldBe(builder.Student.Id);
    }
}
