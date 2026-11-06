using System.Globalization;
using System.Text;
using System.Text.Json;
using ClassManager.Core.Domain.OnlinePayments;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.OnlinePayments;
using ClassManager.Core.UseCases.StudentApp;
using ClassManager.Core.UseCases.Students;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class OnlineOrderPaymentRequests
{
    public const string CheckoutSessionCompleted = "checkout.session.completed";
    public const string PaidStatus = "paid";

    private const string JsonMediaType = "application/json";
    private const decimal MinorUnitsPerMajorUnit = 100m;
    private static readonly TimeSpan ProcessingTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    public sealed record OnlineOrderScenario(
        HttpClient Owner,
        HttpClient Family,
        long BusinessId,
        long OrganizationId,
        StudentAppOrderResponse Order,
        string ExternalAccountId) : IDisposable
    {
        public void Dispose()
        {
            Owner.Dispose();
            Family.Dispose();
        }
    }

    public static async Task<OnlineOrderScenario> SeedOnlineOrderScenarioAsync(
        this ApiFixture fixture,
        bool withAddOn = true,
        bool chargesEnabled = true)
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerTokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, ownerCommand);
        var owner = fixture.CreateClientWithToken(ownerTokens.AccessToken);
        var businessId = (await owner.GetCurrentMemberAsync()).BusinessId;
        await fixture.ChangePlanAsync(businessId, PlanCodes.Pro);
        var organizationId = await fixture.OrganizationIdOfAsync(businessId);
        if (withAddOn)
        {
            await fixture.GiveAddOnAsync(organizationId, Features.OnlinePayments);
        }

        var externalAccountId = await fixture.SeedOnlinePaymentAccountAsync(businessId, organizationId, chargesEnabled);
        var client = await owner.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var classPack = await owner.CreateClassPackAsync();
        var studentEmail = StudentAppRequests.UniqueStudentEmail();
        (await owner.PostStudentAppInvitationAsync(client.Id, studentEmail)).EnsureSuccessStatusCode();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(studentEmail));
        var studentTokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;
        var family = fixture.CreateClientWithToken(studentTokens.AccessToken);
        var order = await family.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));
        return new OnlineOrderScenario(owner, family, businessId, organizationId, order, externalAccountId);
    }

    public static async Task<long> OrganizationIdOfAsync(this ApiFixture fixture, long businessId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        return await context.Businesses.Where(business => business.Id == businessId).Select(business => business.OrganizationId).SingleAsync();
    }

    public static async Task<string> SeedOnlinePaymentAccountAsync(this ApiFixture fixture, long businessId, long organizationId, bool chargesEnabled)
    {
        var externalAccountId = "acct_" + Guid.NewGuid().ToString("N");
        await using var context = fixture.CreateDbContext(businessId);
        var account = OnlinePaymentAccount.Create(organizationId, businessId, externalAccountId, BusinessApiFactory.Now);
        account.Update(new OnlinePaymentAccountState(DetailsSubmitted: true, chargesEnabled, chargesEnabled), BusinessApiFactory.Now);
        context.OnlinePaymentAccounts.Add(account);
        await context.SaveChangesAsync();
        var business = await context.Businesses.SingleAsync(candidate => candidate.Id == businessId);
        business.UseOnlinePaymentAccount(account.Id);
        await context.SaveChangesAsync();
        return externalAccountId;
    }

    public static async Task GiveSalesFeeAsync(this ApiFixture fixture, long organizationId, decimal percent)
    {
        await using var context = fixture.CreateDbContext(0L);
        context.SalesFeeOverrides.Add(SalesFeeOverride.Create(organizationId, percent, note: null, BusinessApiFactory.Now));
        await context.SaveChangesAsync();
    }

    public static Task<HttpResponseMessage> PostOrderCheckoutAsync(this HttpClient family, long orderId) =>
        family.PostAsync(new Uri($"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppOrders}/{orderId}{ApiRoutes.Checkout}", UriKind.Relative), null);

    public static async Task<Uri> StartOrderCheckoutAsync(this HttpClient family, long orderId)
    {
        using var response = await family.PostOrderCheckoutAsync(orderId);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<OnlinePaymentRedirectResponse>(ApiRequests.JsonOptions))!.Url;
    }

    public static async Task<HttpResponseMessage> PostCheckoutCompletedAsync(
        this ApiFixture fixture,
        OnlineOrderScenario scenario,
        string externalPaymentId,
        decimal? amount = null)
    {
        var payload = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["id"] = BillingWebhookRequests.NewEventId(),
            ["object"] = "event",
            ["type"] = CheckoutSessionCompleted,
            ["account"] = scenario.ExternalAccountId,
            ["livemode"] = false,
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["data"] = new Dictionary<string, object>
            {
                ["object"] = new Dictionary<string, object>
                {
                    ["id"] = "cs_" + Guid.NewGuid().ToString("N"),
                    ["object"] = "checkout.session",
                    ["mode"] = "payment",
                    ["payment_status"] = PaidStatus,
                    ["payment_intent"] = externalPaymentId,
                    ["amount_total"] = (long)((amount ?? scenario.Order.Balance) * MinorUnitsPerMajorUnit),
                    ["currency"] = scenario.Order.CurrencyCode.ToLowerInvariant(),
                    ["metadata"] = new Dictionary<string, string>
                    {
                        ["business_id"] = scenario.BusinessId.ToString(CultureInfo.InvariantCulture),
                        ["order_id"] = scenario.Order.Id.ToString(CultureInfo.InvariantCulture),
                    },
                },
            },
        });
        using var stripe = fixture.ApiFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ApiRoutes.StripeConnectWebhook, UriKind.Relative))
        {
            Content = new StringContent(payload, Encoding.UTF8, JsonMediaType),
        };
        request.Headers.Add(BillingWebhookRequests.StripeSignatureHeaderName, BillingWebhookRequests.Signature(payload, BusinessApiFactory.TestStripeConnectWebhookSecret));
        var response = await stripe.SendAsync(request);
        await fixture.WaitUntilNoTaskMentionsAsync(externalPaymentId);
        return response;
    }

    public static async Task WaitUntilNoTaskMentionsAsync(this ApiFixture fixture, string text)
    {
        var deadline = DateTimeOffset.UtcNow + ProcessingTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            await using var context = fixture.CreateDbContext(0L);
            if (!await context.BackgroundTasks.AnyAsync(task => task.Payload.Contains(text)))
            {
                return;
            }

            await Task.Delay(PollInterval);
        }

        throw new TimeoutException($"A background task about {text} is still queued.");
    }

    public static async Task<StudentAppOrderResponse> FamilyOrderAsync(this OnlineOrderScenario scenario) =>
        (await scenario.Family.ListStudentAppOrdersAsync()).Single(order => order.Id == scenario.Order.Id);
}
