namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_valid_link;

public sealed class Then_the_link_is_used_up
{
    [Fact]
    public async Task Then_the_link_is_used_up_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.PendingSignUp.IsPendingAt(TestData.Now).ShouldBeFalse();
    }
}
