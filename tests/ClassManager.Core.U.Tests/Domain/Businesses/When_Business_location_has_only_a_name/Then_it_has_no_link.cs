namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_location_has_only_a_name;

public sealed class Then_it_has_no_link
{
    private const string LocationName = "Av. Costanera 1450, Ciudad";

    [Fact]
    public void Then_it_has_no_link_Run()
    {
        var business = TestData.Business();

        var result = business.ChangeLocation(LocationName, "   ");

        result.IsSuccess.ShouldBeTrue();
        business.LocationName.ShouldBe(LocationName);
        business.LocationMapsUrl.ShouldBeNull();
    }
}
