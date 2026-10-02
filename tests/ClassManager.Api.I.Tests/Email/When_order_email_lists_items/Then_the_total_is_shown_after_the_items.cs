using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.I.Tests.Email.When_order_email_lists_items;

public sealed class Then_the_total_is_shown_after_the_items
{
    [Fact]
    public void Then_the_total_is_shown_after_the_items_Run()
    {
        var content = new EmailContent("Tu pedido", "Recibimos tu pago", "Intro", "Footer")
        {
            Lines = [new EmailLine("3 Remeras", "54,00 EUR")],
            Total = "54,00 EUR",
        };

        var html = BrandedEmailHtml.Render(content, EmailBrand.App);

        html.IndexOf("3 Remeras", StringComparison.Ordinal).ShouldBeLessThan(html.IndexOf(EmailContent.TotalLabel + "</td>", StringComparison.Ordinal));
    }
}
