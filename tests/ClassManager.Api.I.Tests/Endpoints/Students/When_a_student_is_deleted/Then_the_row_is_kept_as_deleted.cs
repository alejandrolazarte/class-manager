using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_row_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_row_is_kept_as_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var client = await owner.RegisterClientAsync();
        var student = await owner.AddStudentAsync(client.Id, "Martina Pérez");

        using var response = await owner.DeleteStudentAsync(student.Id);

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var deletedStudent = await context.Students
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.Id == student.Id);
        deletedStudent.IsDeleted.ShouldBeTrue();
    }
}
