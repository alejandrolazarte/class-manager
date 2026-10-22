using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Api.I.Tests.Persistence.When_two_entities_are_saved_together;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_gets_its_own_id(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_gets_its_own_id_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var first = Instructor.Create("Ana Gómez", BusinessApiFactory.Now).Value!;
        var second = Instructor.Create("Luis Díaz", BusinessApiFactory.Now).Value!;

        context.Instructors.AddRange(first, second);
        await context.SaveChangesAsync();

        (first.Id > 0, second.Id > first.Id).ShouldBe((true, true));
    }
}
