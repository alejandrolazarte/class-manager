using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Options;

namespace ClassManager.Backoffice.Infrastructure.Brands;

internal sealed class BrandBillingLinks(IWebAppLinks webAppLinks, IOptions<BackofficeOptions> options) : IBrandBillingLinks
{
    public const string CustomersPath = "customers/";

    public string PlanOfferPayment() => webAppLinks.PlanOfferPayment();

    public string Customer(string externalCustomerId) =>
        options.Value.StripeDashboardUrl.TrimEnd('/') + "/" + CustomersPath + Uri.EscapeDataString(externalCustomerId);
}
