using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_location_name_is_too_long;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = TestData.Business();

        var result = business.ChangeLocation(new string('a', Business.LocationNameMaxLength + 1), null);

        result.Error!.FieldName.ShouldBe(nameof(Business.LocationName));
    }
}
