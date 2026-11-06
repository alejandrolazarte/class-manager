
namespace ClassManager.Api.I.Tests.Endpoints.EmailUnsubscribe.When_a_student_gets_an_order_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_footer_links_to_stop_these_emails(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_footer_links_to_stop_these_emails_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var email = await fixture.SendOrderReadyEmailAsync(scenario);

        email.HtmlBody.ShouldContain("/unsubscribe?token=");
        email.HtmlBody.ShouldContain(">Dejar de recibir estos emails</a>");
    }
}
