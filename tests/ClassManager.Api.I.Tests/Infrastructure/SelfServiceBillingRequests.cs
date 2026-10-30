using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Subscriptions;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SelfServiceBillingRequests
{
    public const string MadridTimeZoneId = "Europe/Madrid";
    public const string Euro = "EUR";

    private const int DaysSinceTheTrialStarted = 40;

    public sealed record SignedUpBrand(HttpClient HttpClient, long OrganizationId, long BusinessId);

    public static async Task<SignedUpBrand> SignUpBrandAsync(
        this ApiFixture fixture,
        string countryCode = ApiFixture.SpainCountryCode,
        bool withSelfServiceBilling = true)
    {
        var businessName = AuthenticationRequests.UniqueBusinessName();
        var isSpain = countryCode == ApiFixture.SpainCountryCode;
        var command = new RequestSignUpCommand(
            AuthenticationRequests.OwnerFullName,
            AuthenticationRequests.UniqueEmail(),
            businessName,
            countryCode,
            isSpain ? MadridTimeZoneId : ApiFixture.BuenosAiresTimeZoneId,
            isSpain ? Euro : ApiFixture.CurrencyCode,
            AuthenticationRequests.AdultBirthDate,
            AcceptsLegalTerms: true);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        var tokens = await anonymousClient.SignUpAsync(fixture.ApiFactory.EmailTransport, command);

        await using var context = fixture.CreateDbContext(0L);
        var business = await context.Businesses.AsNoTracking().SingleAsync(candidate => candidate.Name == businessName);
        if (withSelfServiceBilling)
        {
            await fixture.GiveSelfServiceBillingAsync(business.OrganizationId);
        }

        return new SignedUpBrand(fixture.CreateClientWithToken(tokens.AccessToken), business.OrganizationId, business.Id);
    }

    public static async Task GiveSelfServiceBillingAsync(this ApiFixture fixture, long organizationId)
    {
        await using var context = fixture.CreateDbContext(0L);
        var subscriptionId = await context.Subscriptions
            .Where(subscription => subscription.SubscriberId == organizationId && !subscription.IsDeleted)
            .Select(subscription => subscription.Id)
            .SingleAsync();
        context.SubscriptionFeatures.Add(SubscriptionFeature.Create(
            subscriptionId,
            Features.SelfServiceBilling,
            price: 0m,
            Euro,
            limit: null,
            expiredOn: null,
            BusinessApiFactory.Now));
        await context.SaveChangesAsync();
    }

    public static async Task EndTrialAsync(this ApiFixture fixture, long organizationId)
    {
        var yesterday = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(-1);
        await using var context = fixture.CreateDbContext(0L);
        var current = await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == organizationId && !subscription.IsDeleted);
        current.Delete(BusinessApiFactory.Now);
        var endedTrial = Subscription.Start(
            organizationId,
            PlanCodes.Free,
            price: 0m,
            Euro,
            note: null,
            BusinessApiFactory.Now.AddDays(-DaysSinceTheTrialStarted));
        endedTrial.Expire(yesterday);
        context.Subscriptions.Add(endedTrial);
        await context.SaveChangesAsync();
    }

    public static Task<HttpResponseMessage> PostPlanCheckoutAsync(this HttpClient httpClient, string planCode) =>
        httpClient.PostAsJsonAsync(ApiRoutes.OrganizationSubscriptionCheckout, new StartPlanCheckoutCommand(planCode), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostBillingPortalAsync(this HttpClient httpClient) =>
        httpClient.PostAsync(new Uri(ApiRoutes.OrganizationSubscriptionPortal, UriKind.Relative), content: null);

    public static async Task<BillingRedirectResponse> ReadRedirectAsync(this HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<BillingRedirectResponse>(ApiRequests.JsonOptions))!;
}
