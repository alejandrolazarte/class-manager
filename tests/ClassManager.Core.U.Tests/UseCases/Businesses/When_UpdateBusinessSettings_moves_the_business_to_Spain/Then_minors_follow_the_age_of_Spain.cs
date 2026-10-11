namespace ClassManager.Core.U.Tests.UseCases.Businesses.When_UpdateBusinessSettings_moves_the_business_to_Spain;

public sealed class Then_minors_follow_the_age_of_Spain
{
    [Fact]
    public async Task Then_minors_follow_the_age_of_Spain_Run()
    {
        var builder = new UpdateBusinessSettingsUseCaseBuilder();

        await builder.Build().ExecuteAsync(builder.CommandFor(TestData.SpainCountryCode, TestData.MadridTimeZoneId), CancellationToken.None);

        builder.Business.Country.OwnAccountMinimumAge.ShouldBe(TestData.SpainOwnAccountMinimumAge);
    }
}
