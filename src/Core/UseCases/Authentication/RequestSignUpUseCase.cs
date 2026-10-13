using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record RequestSignUpCommand(
    string? OwnerFullName,
    string? Email,
    string? BusinessName,
    string? CountryCode,
    string? TimeZoneId,
    string? CurrencyCode,
    DateOnly? OwnerBirthDate,
    bool AcceptsLegalTerms = false) : ICommand;

public sealed record RequestSignUpResponse;

public sealed class RequestSignUpUseCase(
    IIdentityService identityService,
    IPendingSignUpRepository pendingSignUpRepository,
    IBusinessRepository businessRepository,
    ICountryRepository countryRepository,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<RequestSignUpCommand, RequestSignUpResponse>
{
    public const string EmailSubject = "Confirmá tu email";
    public const string AccountExistsEmailSubject = "Ya tenés una cuenta";

    public static EmailContent EmailContentFor(string businessName, string completeSignUpLink) =>
        new(
            "Tu cuenta",
            EmailSubject,
            $"Pediste crear la cuenta de {businessName}. Tocá el botón para confirmar tu email y elegir tu contraseña.",
            "Si no lo pediste, ignorá este mail: no se crea ninguna cuenta.")
        {
            Action = new EmailAction("Confirmar email", completeSignUpLink, ShowsLinkFallback: true),
            Note = new EmailNote("El link sirve una sola vez y vence en 24 horas."),
        };

    public static EmailContent AccountExistsEmailContentFor(string signInLink) =>
        new(
            "Tu cuenta",
            AccountExistsEmailSubject,
            "Alguien pidió crear una cuenta nueva con este email, pero ya tenés una. Entrá con tu email y tu contraseña.",
            "Si no lo pediste, ignorá este mail: tu cuenta no cambia.")
        {
            Action = new EmailAction("Ingresar", signInLink, ShowsLinkFallback: true),
            Note = new EmailNote("Si no recordás la contraseña, tocá \"¿Olvidaste tu contraseña?\" al ingresar."),
        };

    public async Task<Result<RequestSignUpResponse>> ExecuteAsync(RequestSignUpCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var owner = OwnerProfile.Create(command.OwnerFullName, command.Email, command.OwnerBirthDate, today);
        if (owner.IsFailure)
        {
            return NewBrand.RenameField(
                NewBrand.RenameField(owner.Error!, nameof(OwnerProfile.FullName), nameof(RequestSignUpCommand.OwnerFullName)),
                nameof(OwnerProfile.BirthDate),
                nameof(RequestSignUpCommand.OwnerBirthDate));
        }

        if (!PersonAge.IsAdult(owner.Value!.BirthDate, today))
        {
            return Result.Validation<RequestSignUpResponse>(
                AuthenticationErrorCodes.OwnerMustBeAdultMessage,
                AuthenticationErrorCodes.OwnerMustBeAdult,
                nameof(RequestSignUpCommand.OwnerBirthDate));
        }

        var brand = await NewBrand.CreateAsync(
            command.BusinessName,
            command.CountryCode,
            command.TimeZoneId,
            command.CurrencyCode,
            businessRepository,
            countryRepository,
            now,
            cancellationToken);
        if (brand.IsFailure)
        {
            return brand.Error!;
        }

        if (!command.AcceptsLegalTerms)
        {
            return LegalTermsNotAccepted.For<RequestSignUpResponse>(nameof(RequestSignUpCommand.AcceptsLegalTerms));
        }

        if (await identityService.IsEmailRegisteredAsync(owner.Value.Email, cancellationToken))
        {
            await emailSender.SendAsync(
                new EmailMessage(owner.Value.Email, AccountExistsEmailSubject, AccountExistsEmailContentFor(webAppLinks.SignIn())),
                cancellationToken);
            return new RequestSignUpResponse();
        }

        foreach (var previousSignUp in await pendingSignUpRepository.ListPendingForUpdateByEmailAsync(owner.Value.Email, now, cancellationToken))
        {
            previousSignUp.Revoke(now);
        }

        var token = secretTokenGenerator.Create();
        var pendingSignUp = PendingSignUp.Start(owner.Value, brand.Value!.Requested, token.Hash, now, LegalTerms.CurrentVersion);
        pendingSignUpRepository.Add(pendingSignUp);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailSender.SendAsync(
            new EmailMessage(
                pendingSignUp.Email,
                EmailSubject,
                EmailContentFor(pendingSignUp.BusinessName, webAppLinks.CompleteSignUp(token.Value))),
            cancellationToken);

        return new RequestSignUpResponse();
    }
}
