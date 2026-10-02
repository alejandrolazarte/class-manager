using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ClassManager.Notifications.U.Tests.WebPush.When_a_vapid_token_is_created;

public sealed class Then_it_is_signed_for_the_push_service
{
    private const string Endpoint = "https://fcm.googleapis.com/fcm/send/abc123";
    private const string Subject = "mailto:hola@example.com";

    [Fact]
    public void Then_it_is_signed_for_the_push_service_Run()
    {
        var keys = VapidKeys.Generate();
        var options = new VapidOptions { Subject = Subject, PublicKey = keys.PublicKey, PrivateKey = keys.PrivateKey };

        var header = VapidAuthorization.Create(options, new Uri(Endpoint), NotificationsTestData.Now);

        header.ShouldStartWith("vapid t=");
        header.ShouldEndWith($", k={keys.PublicKey}");
        var token = header["vapid t=".Length..header.IndexOf(',', StringComparison.Ordinal)];
        var parts = token.Split('.');
        using var verifier = EcKeys.ImportSigningPublicKey(keys.PublicKey);
        verifier.VerifyData(Encoding.ASCII.GetBytes($"{parts[0]}.{parts[1]}"), Base64Url.DecodeFromChars(parts[2]), HashAlgorithmName.SHA256).ShouldBeTrue();
        using var claims = JsonDocument.Parse(Base64Url.DecodeFromChars(parts[1]));
        claims.RootElement.GetProperty("aud").GetString().ShouldBe("https://fcm.googleapis.com");
        claims.RootElement.GetProperty("sub").GetString().ShouldBe(Subject);
    }
}
