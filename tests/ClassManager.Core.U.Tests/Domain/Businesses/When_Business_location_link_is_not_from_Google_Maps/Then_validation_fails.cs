using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_location_link_is_not_from_Google_Maps;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = TestData.Business();

        var result = business.ChangeLocation("Av. Costanera 1450, Ciudad", "https://www.example.com/maps/sede");

        result.Error!.FieldName.ShouldBe(nameof(Business.LocationMapsUrl));
        business.LocationName.ShouldBeNull();
    }
}
