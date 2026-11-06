using System.Text;
using System.Text.Json;
using ClassManager.Core.Domain.OnlinePayments;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.OnlinePayments;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class OnlinePaymentRequests
{
    public const string AccountUpdated = "account.updated";
    public const string OnboardingReturnedUrl = "http://localhost:8081/settings/online-payments?onboarding=return";
    public const string OnboardingExpiredUrl = "http://localhost:8081/settings/online-payments?onboarding=refresh";

    private const string JsonMediaType = "application/json";

    public sealed record BrandWithTwoBranches(SelfServiceBillingRequests.SignedUpBrand Brand, HttpClient SecondBranch, long SecondBusinessId, string SecondBranchName);

    public static async Task<BrandWithTwoBranches> SignUpBrandWithTwoBranchesAsync(this ApiFixture fixture)
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.ChangePlanAsync(brand.BusinessId, PlanCodes.Enterprise);
        var secondBranchName = AuthenticationRequests.UniqueBusinessName();
        var secondBranch = await brand.HttpClient.CreateBranchAsync(secondBranchName);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        var tokens = await anonymousClient.SwitchBranchAsync(brand.RefreshToken, secondBranch.BusinessId);
        return new BrandWithTwoBranches(brand, fixture.CreateClientWithToken(tokens.AccessToken), secondBranch.BusinessId, secondBranchName);
    }

    public static Task<HttpResponseMessage> GetOnlinePaymentsAsync(this HttpClient httpClient) =>
        httpClient.GetAsync(new Uri(ApiRoutes.OnlinePayments, UriKind.Relative));

    public static async Task<OnlinePaymentsResponse> ReadOnlinePaymentsAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.GetOnlinePaymentsAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<OnlinePaymentsResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostOnlinePaymentOnboardingAsync(this HttpClient httpClient) =>
        httpClient.PostAsync(new Uri(ApiRoutes.OnlinePaymentsOnboarding, UriKind.Relative), null);

    public static async Task<Uri> StartOnlinePaymentOnboardingAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.PostOnlinePaymentOnboardingAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<OnlinePaymentRedirectResponse>(ApiRequests.JsonOptions))!.Url;
    }

    public static Task<HttpResponseMessage> PostOnlinePaymentRefreshAsync(this HttpClient httpClient) =>
        httpClient.PostAsync(new Uri(ApiRoutes.OnlinePaymentsRefresh, UriKind.Relative), null);

    public static Task<HttpResponseMessage> PutOnlinePaymentAccountAsync(this HttpClient httpClient, long accountId) =>
        httpClient.PutAsJsonAsync(new Uri(ApiRoutes.OnlinePaymentsAccount, UriKind.Relative), new { accountId = accountId.ToString(System.Globalization.CultureInfo.InvariantCulture) }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteOnlinePaymentAccountAsync(this HttpClient httpClient) =>
        httpClient.DeleteAsync(new Uri(ApiRoutes.OnlinePaymentsAccount, UriKind.Relative));

    public static async Task<OnlinePaymentAccount> OnlinePaymentAccountOfAsync(this ApiFixture fixture, long businessId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var accountId = await context.Businesses.Where(business => business.Id == businessId).Select(business => business.OnlinePaymentAccountId).SingleAsync();
        return await context.OnlinePaymentAccounts.AsNoTracking().SingleAsync(account => account.Id == accountId);
    }

    public static async Task<HttpResponseMessage> PostStripeConnectEventAsync(
        this ApiFixture fixture,
        string eventType,
        string externalAccountId,
        string webhookSecret = BusinessApiFactory.TestStripeConnectWebhookSecret)
    {
        var payload = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["id"] = BillingWebhookRequests.NewEventId(),
            ["object"] = "event",
            ["type"] = eventType,
            ["account"] = externalAccountId,
            ["livemode"] = false,
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["data"] = new Dictionary<string, object>
            {
                ["object"] = new Dictionary<string, object> { ["id"] = externalAccountId, ["object"] = "account" },
            },
        });
        using var stripe = fixture.ApiFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ApiRoutes.StripeConnectWebhook, UriKind.Relative))
        {
            Content = new StringContent(payload, Encoding.UTF8, JsonMediaType),
        };
        request.Headers.Add(BillingWebhookRequests.StripeSignatureHeaderName, BillingWebhookRequests.Signature(payload, webhookSecret));
        return await stripe.SendAsync(request);
    }
}
