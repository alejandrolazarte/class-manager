using ClassManager.Infrastructure.WebPush;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.WebPush.When_the_push_service_answers_gone;

public sealed class Then_the_subscription_is_reported_gone
{
    [Fact]
    public async Task Then_the_subscription_is_reported_gone_Run()
    {
        var keys = VapidKeys.Generate();
        var options = Options.Create(new VapidOptions { Subject = "mailto:hola@example.com", PublicKey = keys.PublicKey, PrivateKey = keys.PrivateKey });
        using var handler = new CapturingHandler(HttpStatusCode.Gone);
        using var httpClient = new HttpClient(handler);
        var sender = new WebPushSender(httpClient, options, new FakeTimeProvider(BusinessApiFactory.Now), NullLogger<WebPushSender>.Instance);
        var subscription = PushRequests.NewSubscription();

        var delivery = await sender.SendAsync(
            new PushTarget(subscription.Endpoint!, subscription.P256dh!, subscription.Auth!), "{}", CancellationToken.None);

        delivery.ShouldBe(PushDelivery.Gone);
        handler.Request!.Headers.GetValues("TTL").ShouldHaveSingleItem().ShouldBe("86400");
        handler.Request.Headers.GetValues("Authorization").ShouldHaveSingleItem().ShouldStartWith("vapid t=");
        handler.ContentEncoding.ShouldBe("aes128gcm");
    }

    private sealed class CapturingHandler(HttpStatusCode statusCode) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        public string? ContentEncoding { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            ContentEncoding = request.Content?.Headers.ContentEncoding.SingleOrDefault();
            return Task.FromResult(new HttpResponseMessage(statusCode));
        }
    }
}
