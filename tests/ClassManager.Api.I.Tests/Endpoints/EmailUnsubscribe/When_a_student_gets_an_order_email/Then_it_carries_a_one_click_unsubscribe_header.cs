
namespace ClassManager.Api.I.Tests.Endpoints.EmailUnsubscribe.When_a_student_gets_an_order_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_carries_a_one_click_unsubscribe_header(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_carries_a_one_click_unsubscribe_header_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var email = await fixture.SendOrderReadyEmailAsync(scenario);

        email.OneClickUnsubscribeUrl.ShouldStartWith(BusinessApiFactory.TestApiPublicUrl + ApiRoutes.EmailUnsubscribe + "?token=");
    }
}
