using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Security.Accounts;
using ClassManager.Security.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClassManager.Security.Passkeys;

internal sealed class PasskeyService(
    SecurityDbContext securityContext,
    UserManager<ApplicationUser> userManager,
    IPasskeyHandler<ApplicationUser> passkeyHandler,
    IHttpContextAccessor httpContextAccessor,
    IOptions<PasskeyCeremonyOptions> ceremonyOptions,
    TimeProvider timeProvider)
    : IPasskeyService
{
    private const int CeremonyIdByteCount = 32;
    private const string MissingHttpContextMessage = "Passkey ceremonies run inside an HTTP request.";

    public async Task<PasskeyRegistrationStart> BeginRegistrationAsync(
        Guid userId,
        string currentPassword,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return PasskeyRegistrationStart.UnknownUser;
        }

        if (!await userManager.CheckPasswordAsync(user, currentPassword))
        {
            return PasskeyRegistrationStart.InvalidPassword;
        }

        var userEntity = new PasskeyUserEntity
        {
            Id = await userManager.GetUserIdAsync(user),
            Name = user.Email ?? string.Empty,
            DisplayName = user.FullName,
        };
        var options = await passkeyHandler.MakeCreationOptionsAsync(userEntity, CurrentHttpContext());
        var ceremonyId = await StoreCeremonyAsync(PasskeyCeremonyKind.Registration, user.Id, options.AttestationState, cancellationToken);

        return PasskeyRegistrationStart.Started(new PasskeyCeremonyStart(ceremonyId, options.CreationOptionsJson));
    }

    public async Task<PasskeyRegistration> FinishRegistrationAsync(
        Guid userId,
        string ceremonyId,
        string credentialJson,
        string name,
        CancellationToken cancellationToken)
    {
        var ceremony = await TakeCeremonyAsync(ceremonyId, PasskeyCeremonyKind.Registration, cancellationToken);
        if (ceremony is null || ceremony.UserId != userId)
        {
            return PasskeyRegistration.InvalidCeremony;
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return PasskeyRegistration.InvalidCeremony;
        }

        var attestation = await passkeyHandler.PerformAttestationAsync(new PasskeyAttestationContext
        {
            CredentialJson = credentialJson,
            AttestationState = ceremony.State,
            HttpContext = CurrentHttpContext(),
        });
        if (!attestation.Succeeded || attestation.UserEntity.Id != await userManager.GetUserIdAsync(user))
        {
            return PasskeyRegistration.InvalidCredential;
        }

        if (await userManager.FindByPasskeyIdAsync(attestation.Passkey.CredentialId) is not null)
        {
            return PasskeyRegistration.InvalidCredential;
        }

        attestation.Passkey.Name = name;
        var addition = await userManager.AddOrUpdatePasskeyAsync(user, attestation.Passkey);
        if (!addition.Succeeded)
        {
            return PasskeyRegistration.InvalidCredential;
        }

        return PasskeyRegistration.Registered(Summary(attestation.Passkey), user.Email ?? string.Empty);
    }

    public async Task<PasskeyCeremonyStart> BeginSignInAsync(CancellationToken cancellationToken)
    {
        var options = await passkeyHandler.MakeRequestOptionsAsync(null, CurrentHttpContext());
        var ceremonyId = await StoreCeremonyAsync(PasskeyCeremonyKind.SignIn, null, options.AssertionState, cancellationToken);

        return new PasskeyCeremonyStart(ceremonyId, options.RequestOptionsJson);
    }

    public async Task<CredentialCheck> SignInAsync(string ceremonyId, string credentialJson, CancellationToken cancellationToken)
    {
        var ceremony = await TakeCeremonyAsync(ceremonyId, PasskeyCeremonyKind.SignIn, cancellationToken);
        if (ceremony is null)
        {
            return CredentialCheck.InvalidCredentials;
        }

        var assertion = await passkeyHandler.PerformAssertionAsync(new PasskeyAssertionContext
        {
            CredentialJson = credentialJson,
            AssertionState = ceremony.State,
            HttpContext = CurrentHttpContext(),
        });
        if (!assertion.Succeeded)
        {
            return CredentialCheck.InvalidCredentials;
        }

        var user = assertion.User;
        if (await userManager.IsLockedOutAsync(user))
        {
            return CredentialCheck.LockedOut;
        }

        await userManager.AddOrUpdatePasskeyAsync(user, assertion.Passkey);

        return CredentialCheck.Verified(user.Id, user.Email ?? string.Empty);
    }

    public async Task<IReadOnlyList<UserPasskeySummary>> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return [];
        }

        var passkeys = await userManager.GetPasskeysAsync(user);

        return [.. passkeys.OrderBy(passkey => passkey.CreatedAt).Select(Summary)];
    }

    public async Task<bool> RemoveAsync(Guid userId, string passkeyId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || !TryDecodePasskeyId(passkeyId, out var credentialId))
        {
            return false;
        }

        if (await userManager.GetPasskeyAsync(user, credentialId) is null)
        {
            return false;
        }

        return (await userManager.RemovePasskeyAsync(user, credentialId)).Succeeded;
    }

    private async Task<string> StoreCeremonyAsync(
        PasskeyCeremonyKind kind,
        Guid? userId,
        string? state,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        await securityContext.PasskeyCeremonies
            .Where(ceremony => ceremony.ExpiresAt <= now)
            .ExecuteDeleteAsync(cancellationToken);

        var ceremonyId = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(CeremonyIdByteCount));
        securityContext.PasskeyCeremonies.Add(new PasskeyCeremony
        {
            Id = Guid.CreateVersion7(),
            CeremonyIdHash = Hash(ceremonyId),
            Kind = kind,
            UserId = userId,
            State = state,
            CreatedAt = now,
            ExpiresAt = now + ceremonyOptions.Value.Lifetime,
        });
        await securityContext.SaveChangesAsync(cancellationToken);

        return ceremonyId;
    }

    private async Task<PasskeyCeremony?> TakeCeremonyAsync(string ceremonyId, PasskeyCeremonyKind kind, CancellationToken cancellationToken)
    {
        var ceremonyIdHash = Hash(ceremonyId);
        var ceremony = await securityContext.PasskeyCeremonies
            .AsNoTracking()
            .SingleOrDefaultAsync(storedCeremony => storedCeremony.CeremonyIdHash == ceremonyIdHash, cancellationToken);
        if (ceremony is null)
        {
            return null;
        }

        var takenCount = await securityContext.PasskeyCeremonies
            .Where(storedCeremony => storedCeremony.Id == ceremony.Id)
            .ExecuteDeleteAsync(cancellationToken);
        var isUsable = takenCount == 1 && ceremony.Kind == kind && ceremony.ExpiresAt > timeProvider.GetUtcNow();

        return isUsable ? ceremony : null;
    }

    private HttpContext CurrentHttpContext() =>
        httpContextAccessor.HttpContext ?? throw new InvalidOperationException(MissingHttpContextMessage);

    private static UserPasskeySummary Summary(UserPasskeyInfo passkey) =>
        new(Base64Url.EncodeToString(passkey.CredentialId), passkey.Name ?? string.Empty, passkey.CreatedAt);

    private static bool TryDecodePasskeyId(string passkeyId, out byte[] credentialId)
    {
        credentialId = [];
        if (string.IsNullOrEmpty(passkeyId) || !Base64Url.IsValid(passkeyId))
        {
            return false;
        }

        credentialId = Base64Url.DecodeFromChars(passkeyId);
        return true;
    }

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
