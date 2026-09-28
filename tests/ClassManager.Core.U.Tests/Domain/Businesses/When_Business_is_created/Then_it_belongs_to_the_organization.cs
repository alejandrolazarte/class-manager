using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_is_created;

public sealed class Then_it_belongs_to_the_organization
{
    [Fact]
    public void Then_it_belongs_to_the_organization_Run()
    {
        var organizationId = Guid.CreateVersion7();

        var business = Business.Create(
            organizationId,
            TestData.BusinessName,
            "panaderia-laura",
            TestData.BuenosAiresTimeZoneId,
            TestData.CurrencyCode,
            TestData.DefaultCountryCallingCode,
            TestData.Now);

        business.Value!.OrganizationId.ShouldBe(organizationId);
    }
}
