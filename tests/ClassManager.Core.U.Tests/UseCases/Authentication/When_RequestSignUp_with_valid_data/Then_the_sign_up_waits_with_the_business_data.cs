namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_with_valid_data;

public sealed class Then_the_sign_up_waits_with_the_business_data
{
    [Fact]
    public async Task Then_the_sign_up_waits_with_the_business_data_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(RequestSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedPendingSignUp!.BusinessName.ShouldBe(TestData.BusinessName);
        builder.AddedPendingSignUp.TokenHash.ShouldBe(RequestSignUpUseCaseBuilder.TokenHash);
        builder.AddedPendingSignUp.IsPendingAt(TestData.Now).ShouldBeTrue();
    }
}
