using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.U.Tests.Domain.Organizations.When_Organization_is_created_with_short_name;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var organization = Organization.Create(" A ", TestData.Now);

        organization.IsFailure.ShouldBeTrue();
        organization.Error!.FieldName.ShouldBe(nameof(Organization.Name));
    }
}
