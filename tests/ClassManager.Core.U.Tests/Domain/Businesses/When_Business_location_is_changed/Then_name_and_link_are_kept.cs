namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_location_is_changed;

public sealed class Then_name_and_link_are_kept
{
    private const string LocationName = "Av. Costanera 1450, Ciudad";
    private const string MapsUrl = "https://maps.app.goo.gl/k3Hq8vPzR2";

    [Fact]
    public void Then_name_and_link_are_kept_Run()
    {
        var business = TestData.Business();

        var result = business.ChangeLocation($"  {LocationName} ", $" {MapsUrl} ");

        result.IsSuccess.ShouldBeTrue();
        business.LocationName.ShouldBe(LocationName);
        business.LocationMapsUrl.ShouldBe(MapsUrl);
    }
}
