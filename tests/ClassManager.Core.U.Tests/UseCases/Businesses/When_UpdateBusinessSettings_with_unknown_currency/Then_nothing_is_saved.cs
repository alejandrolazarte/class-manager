using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.Businesses.When_UpdateBusinessSettings_with_unknown_currency;

public sealed class Then_nothing_is_saved
{
    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new UpdateBusinessSettingsUseCaseBuilder();
        var command = builder.CommandWithTimeZone(TestData.BuenosAiresTimeZoneId) with { CurrencyCode = "XYZ" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        response.Error!.FieldName.ShouldBe(nameof(UpdateBusinessSettingsCommand.CurrencyCode));
    }
}
