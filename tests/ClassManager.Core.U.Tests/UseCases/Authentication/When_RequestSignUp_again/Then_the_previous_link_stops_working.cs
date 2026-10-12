namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_again;

public sealed class Then_the_previous_link_stops_working
{
    [Fact]
    public async Task Then_the_previous_link_stops_working_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();
        var previousSignUp = TestData.PendingSignUp("previous-token-hash", TestData.Now.AddHours(-1));
        builder.PreviousSignUps.Add(previousSignUp);

        await builder.Build().ExecuteAsync(RequestSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        previousSignUp.IsPendingAt(TestData.Now).ShouldBeFalse();
        builder.AddedPendingSignUp!.IsPendingAt(TestData.Now).ShouldBeTrue();
    }
}
