namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_business_slug_is_taken;

public sealed class Then_numeric_suffix_is_added
{
    [Fact]
    public async Task Then_numeric_suffix_is_added_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();
        builder.Businesses
            .Setup(repository => repository.IsSlugTakenAsync("panaderia-laura", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await builder.Build().ExecuteAsync(SignUpOwnerUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedBusiness!.Slug.ShouldBe("panaderia-laura-2");
    }
}
