namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_and_the_business_slug_is_taken;

public sealed class Then_numeric_suffix_is_added
{
    [Fact]
    public async Task Then_numeric_suffix_is_added_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();
        builder.Businesses
            .Setup(repository => repository.IsSlugTakenAsync("panaderia-laura", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedBusiness!.Slug.ShouldBe("panaderia-laura-2");
    }
}
