namespace ClassManager.Backoffice.Core.Brands;

public interface IBrandBillingLinks
{
    string PlanOfferPayment();

    string Customer(string externalCustomerId);
}
