using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_family_has_several_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_home_lists_them_in_the_order_they_were_created(ApiFixture fixture)
{
    private const string SiblingCreatedLaterFullName = "Bruno Pérez";

    [Fact]
    public async Task Then_the_home_lists_them_in_the_order_they_were_created_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var siblingCreatedLater = await business.HttpClient.AddStudentAsync(family.Id, SiblingCreatedLaterFullName);
        await using (var context = fixture.CreateDbContext(business.Business.Id))
        {
            await context.Students
                .Where(student => student.Id == siblingCreatedLater.Id)
                .ExecuteUpdateAsync(student => student.SetProperty(row => row.CreatedAt, BusinessApiFactory.Now.AddDays(1)));
        }

        using var clientApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);

        var home = await clientApp.GetStudentAppHomeAsync();

        home!.Students.Select(student => student.FullName).ShouldBe([StudentAppRequests.ChildFullName, SiblingCreatedLaterFullName]);
    }
}
