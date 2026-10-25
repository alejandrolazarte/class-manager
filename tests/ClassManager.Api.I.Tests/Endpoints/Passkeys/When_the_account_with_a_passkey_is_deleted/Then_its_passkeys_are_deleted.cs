using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_the_account_with_a_passkey_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_passkeys_are_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_passkeys_are_deleted_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var authenticator = new SoftwareAuthenticator();
        await scenario.Student.AddPasskeyAsync(authenticator, StudentAppRequests.StudentAppPassword);
        using (var deletion = await scenario.Student.PostAccountDeletionAsync(StudentAppRequests.StudentAppPassword))
        {
            deletion.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        await using var securityContext = fixture.CreateSecurityDbContext();
        var credentialId = System.Buffers.Text.Base64Url.DecodeFromChars(authenticator.CredentialId);
        (await securityContext.UserPasskeys.AnyAsync(passkey => passkey.CredentialId == credentialId)).ShouldBeFalse();
    }
}
