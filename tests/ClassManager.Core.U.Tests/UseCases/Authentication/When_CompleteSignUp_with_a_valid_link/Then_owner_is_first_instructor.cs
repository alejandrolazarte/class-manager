namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_valid_link;

public sealed class Then_owner_is_first_instructor
{
    [Fact]
    public async Task Then_owner_is_first_instructor_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedInstructor!.FullName.ShouldBe(TestData.OwnerFullName);
    }
}
