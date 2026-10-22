namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_location_is_cleared;

public sealed class Then_name_and_link_are_removed
{
    [Fact]
    public void Then_name_and_link_are_removed_Run()
    {
        var business = TestData.Business();
        business.ChangeLocation("Av. Costanera 1450, Ciudad", "https://maps.app.goo.gl/k3Hq8vPzR2");

        var result = business.ChangeLocation(null, " ");

        result.IsSuccess.ShouldBeTrue();
        business.LocationName.ShouldBeNull();
        business.LocationMapsUrl.ShouldBeNull();
    }
}
