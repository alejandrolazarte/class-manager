using System.Buffers.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.WebPush;

internal sealed partial class WebPushSender(
    HttpClient httpClient,
    IOptions<VapidOptions> options,
    TimeProvider timeProvider,
    ILogger<WebPushSender> logger)
    : IWebPushSender
{
    private const string TimeToLiveHeader = "TTL";
    private const string UrgencyHeader = "Urgency";
    private const string NormalUrgency = "normal";
    private const string ContentEncoding = "aes128gcm";
    private const string OctetStream = "application/octet-stream";
    private const string AuthorizationHeader = "Authorization";

    private static readonly string TimeToLiveSeconds = ((int)TimeSpan.FromDays(1).TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);

    public async Task<PushDelivery> SendAsync(PushTarget target, string payload, CancellationToken cancellationToken)
    {
        byte[] body;
        try
        {
            body = WebPushEncryption.Encrypt(
                Encoding.UTF8.GetBytes(payload), Base64Url.DecodeFromChars(target.P256dh), Base64Url.DecodeFromChars(target.Auth));
        }
        catch (Exception exception) when (exception is CryptographicException or FormatException)
        {
            LogInvalidKeys(logger, exception);
            return PushDelivery.Gone;
        }

        var endpoint = new Uri(target.Endpoint);
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.TryAddWithoutValidation(AuthorizationHeader, VapidAuthorization.Create(options.Value, endpoint, timeProvider.GetUtcNow()));
        request.Headers.Add(TimeToLiveHeader, TimeToLiveSeconds);
        request.Headers.Add(UrgencyHeader, NormalUrgency);
        request.Content = new ByteArrayContent(body);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(OctetStream);
        request.Content.Headers.ContentEncoding.Add(ContentEncoding);
        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return PushDelivery.Delivered;
            }

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
            {
                return PushDelivery.Gone;
            }

            LogRejected(logger, (int)response.StatusCode, endpoint.Host);
            return PushDelivery.Failed;
        }
        catch (HttpRequestException exception)
        {
            LogFailed(logger, exception, endpoint.Host);
            return PushDelivery.Failed;
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "A push subscription has invalid keys")]
    private static partial void LogInvalidKeys(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Push service {Host} answered {StatusCode}")]
    private static partial void LogRejected(ILogger logger, int statusCode, string host);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Sending a push to {Host} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, string host);
}
