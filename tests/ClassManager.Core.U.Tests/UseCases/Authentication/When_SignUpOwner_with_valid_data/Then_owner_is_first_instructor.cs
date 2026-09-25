namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_valid_data;

public sealed class Then_owner_is_first_instructor
{
    [Fact]
    public async Task Then_owner_is_first_instructor_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();

        await builder.Build().ExecuteAsync(SignUpOwnerUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedInstructor!.FullName.ShouldBe(TestData.OwnerFullName);
    }
}
