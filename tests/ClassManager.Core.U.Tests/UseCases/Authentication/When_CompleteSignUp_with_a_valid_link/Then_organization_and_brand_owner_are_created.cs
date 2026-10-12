using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_valid_link;

public sealed class Then_organization_and_brand_owner_are_created
{
    [Fact]
    public async Task Then_organization_and_brand_owner_are_created_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedOrganization!.Name.ShouldBe(TestData.BusinessName);
        builder.AddedBusiness!.OrganizationId.ShouldBe(builder.AddedOrganization.Id);
        builder.AddedOrganizationMember!.OrganizationId.ShouldBe(builder.AddedOrganization.Id);
        builder.AddedOrganizationMember.UserId.ShouldBe(builder.UserId);
        builder.AddedOrganizationMember.Role.ShouldBe(OrganizationRole.BrandOwner);
    }
}
