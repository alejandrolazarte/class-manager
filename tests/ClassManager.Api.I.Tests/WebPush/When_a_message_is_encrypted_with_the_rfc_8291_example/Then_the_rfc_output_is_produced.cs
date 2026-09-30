using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Infrastructure.WebPush;

namespace ClassManager.Api.I.Tests.WebPush.When_a_message_is_encrypted_with_the_rfc_8291_example;

public sealed class Then_the_rfc_output_is_produced
{
    private const string Plaintext = "When I grow up, I want to be a watermelon";
    private const string ApplicationServerPrivateKey = "yfWPiYE-n46HLnH0KqZOF1fJJU3MYrct3AELtAQ-oRw";
    private const string ApplicationServerPublicKey = "BP4z9KsN6nGRTbVYI_c7VJSPQTBtkgcy27mlmlMoZIIgDll6e3vCYLocInmYWAmS6TlzAC8wEqKK6PBru3jl7A8";
    private const string UserAgentPublicKey = "BCVxsr7N_eNgVRqvHtD0zTZsEc6-VV-JvLexhqUzORcxaOzi6-AYWXvTBHm4bjyPjs7Vd8pZGH6SRpkNtoIAiw4";
    private const string Salt = "DGv6ra1nlYgDCS1FRnbzlw";
    private const string AuthSecret = "BTBZMqHH6r4Tts7J_aSIgg";
    private const string ExpectedMessage =
        "DGv6ra1nlYgDCS1FRnbzlwAAEABBBP4z9KsN6nGRTbVYI_c7VJSPQTBtkgcy27mlmlMoZIIgDll6e3vCYLocInmYWAmS6TlzAC8wEqKK6PBru3jl7A_yl95bQpu6cVPTpK4Mqgkf1CXztLVBSt2Ks3oZwbuwXPXLWyouBWLVWGNWQexSgSxsj_Qulcy4a-fN";

    [Fact]
    public void Then_the_rfc_output_is_produced_Run()
    {
        using var applicationServerKey = EcKeys.ImportDiffieHellman(ApplicationServerPrivateKey, ApplicationServerPublicKey);

        var message = WebPushEncryption.Encrypt(
            Encoding.UTF8.GetBytes(Plaintext),
            Base64Url.DecodeFromChars(UserAgentPublicKey),
            Base64Url.DecodeFromChars(AuthSecret),
            applicationServerKey,
            Base64Url.DecodeFromChars(Salt));

        Base64Url.EncodeToString(message).ShouldBe(ExpectedMessage);
    }
}
