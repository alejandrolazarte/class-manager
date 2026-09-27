using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_uploading_a_body_far_over_the_size_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_request_is_rejected_as_a_client_error(ApiFixture fixture)
{
    private const int SizeMultiplier = 3;

    [Fact]
    public async Task Then_request_is_rejected_as_a_client_error_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var csvText = "Profesor\n" + new string('x', ImportLimits.DefaultMaximumFileSizeInBytes * SizeMultiplier);

        using var response = await business.HttpClient.PostImportFileAsync(InstructorImportModule.ModuleName, ApiRoutes.ImportAction, csvText);

        ((int)response.StatusCode).ShouldBeInRange(400, 499);
    }
}
