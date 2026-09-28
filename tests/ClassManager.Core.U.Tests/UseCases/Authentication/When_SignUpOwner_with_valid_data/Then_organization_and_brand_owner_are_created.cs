using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_valid_data;

public sealed class Then_organization_and_brand_owner_are_created
{
    [Fact]
    public async Task Then_organization_and_brand_owner_are_created_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();

        await builder.Build().ExecuteAsync(SignUpOwnerUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedOrganization!.Name.ShouldBe(TestData.BusinessName);
        builder.AddedBusiness!.OrganizationId.ShouldBe(builder.AddedOrganization.Id);
        builder.AddedOrganizationMember!.OrganizationId.ShouldBe(builder.AddedOrganization.Id);
        builder.AddedOrganizationMember.UserId.ShouldBe(builder.UserId);
        builder.AddedOrganizationMember.Role.ShouldBe(OrganizationRole.BrandOwner);
    }
}
