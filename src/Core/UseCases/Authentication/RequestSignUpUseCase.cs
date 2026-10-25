using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Emails;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record RequestSignUpCommand(
    [property: AuditPersonal] string? OwnerFullName,
    [property: AuditPersonal] string? Email,
    [property: AuditPlain] string? BusinessName,
    [property: AuditPlain] string? CountryCode,
    [property: AuditPlain] string? TimeZoneId,
    [property: AuditPlain] string? CurrencyCode,
    [property: AuditPersonal] DateOnly? OwnerBirthDate,
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
            var emailTexts = await RecipientEmailTexts.OfEmailAsync(identityService, owner.Value.Email, cancellationToken);
            var accountExists = emailTexts.SignUpAccountExists(webAppLinks.SignIn());
            await emailSender.SendAsync(new EmailMessage(owner.Value.Email, accountExists.Subject, accountExists.Content), cancellationToken);
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

        var confirmation = EmailTexts.Spanish.SignUpConfirmation(pendingSignUp.BusinessName, webAppLinks.CompleteSignUp(token.Value, pendingSignUp.Email));
        await emailSender.SendAsync(new EmailMessage(pendingSignUp.Email, confirmation.Subject, confirmation.Content), cancellationToken);

        return new RequestSignUpResponse();
    }
}
