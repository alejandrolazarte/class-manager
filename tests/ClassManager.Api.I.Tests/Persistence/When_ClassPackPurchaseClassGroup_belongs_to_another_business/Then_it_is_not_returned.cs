using ClassManager.Core.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClassPackPurchaseClassGroup_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherPack = await otherBusiness.HttpClient.CreateClassPackAsync(classGroupIds: [otherClassGroup.Id]);
        var otherClient = await otherBusiness.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var otherPurchase = await otherBusiness.HttpClient.SellClassPackAsync(otherClient.Id, otherPack.Id);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClassPackPurchaseClassGroups.AnyAsync(purchaseClassGroup => purchaseClassGroup.ClassPackPurchaseId == otherPurchase.Id)).ShouldBeFalse();
    }
}
