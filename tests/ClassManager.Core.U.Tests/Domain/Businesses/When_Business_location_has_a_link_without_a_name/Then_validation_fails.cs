using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_location_has_a_link_without_a_name;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = TestData.Business();

        var result = business.ChangeLocation(" ", "https://maps.app.goo.gl/k3Hq8vPzR2");

        result.Error!.FieldName.ShouldBe(nameof(Business.LocationName));
        business.LocationMapsUrl.ShouldBeNull();
    }
}
