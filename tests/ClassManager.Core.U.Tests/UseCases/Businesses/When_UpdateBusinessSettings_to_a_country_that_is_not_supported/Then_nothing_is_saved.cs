using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.Businesses.When_UpdateBusinessSettings_to_a_country_that_is_not_supported;

public sealed class Then_nothing_is_saved
{
    private const string UnsupportedCountryCode = "FR";

    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new UpdateBusinessSettingsUseCaseBuilder();

        var result = await builder.Build().ExecuteAsync(builder.CommandFor(UnsupportedCountryCode, "Europe/Paris"), CancellationToken.None);

        result.Error!.FieldName.ShouldBe(nameof(Business.CountryCode));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
