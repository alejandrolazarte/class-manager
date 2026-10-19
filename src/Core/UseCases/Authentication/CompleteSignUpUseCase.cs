using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record CompleteSignUpCommand([property: AuditSecret] string? Token, [property: AuditSecret] string? Password) : ICommand;

public sealed class CompleteSignUpUseCase(
    IPendingSignUpRepository pendingSignUpRepository,
    IIdentityService identityService,
    ITokenService tokenService,
    ISecretTokenGenerator secretTokenGenerator,
    IOrganizationRepository organizationRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IBusinessRepository businessRepository,
    ICountryRepository countryRepository,
    IBusinessMemberRepository businessMemberRepository,
    IInstructorRepository instructorRepository,
    ISubscriptionRepository subscriptionRepository,
    ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository,
    IUnitOfWork unitOfWork,
    ITenantScope tenantScope,
    TimeProvider timeProvider)
    : IUseCase<CompleteSignUpCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(CompleteSignUpCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return InvalidLink();
        }

        var now = timeProvider.GetUtcNow();
        var pendingSignUp = await pendingSignUpRepository.FindForUpdateByTokenHashAsync(
            secretTokenGenerator.Hash(command.Token),
            cancellationToken);
        if (pendingSignUp is null || !pendingSignUp.IsPendingAt(now))
        {
            return InvalidLink();
        }

        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var account = OwnerAccount.Create(
            pendingSignUp.OwnerFullName,
            pendingSignUp.Email,
            command.Password,
            pendingSignUp.OwnerBirthDate,
            today);
        if (account.IsFailure)
        {
            return account.Error!;
        }

        var requested = pendingSignUp.Business;
        var brand = await NewBrand.CreateAsync(
            requested.Name,
            requested.CountryCode,
            requested.TimeZoneId,
            requested.CurrencyCode,
            businessRepository,
            countryRepository,
            now,
            cancellationToken);
        if (brand.IsFailure)
        {
            return brand.Error!;
        }

        if (await identityService.IsEmailRegisteredAsync(account.Value!.Email, cancellationToken))
        {
            return Result.Conflict<TokenResponse>(AuthenticationErrorCodes.EmailTakenMessage, AuthenticationErrorCodes.EmailTaken);
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var userId = await identityService.CreateOwnerAsync(account.Value, cancellationToken);
        if (userId.IsFailure)
        {
            return userId.Error!;
        }

        var (organization, business) = brand.Value!;
        tenantScope.Establish(business.Id);
        organizationRepository.Add(organization);
        organizationMemberRepository.Add(OrganizationMember.CreateBrandOwner(organization.Id, userId.Value));
        businessRepository.Add(business);
        var ownerInstructor = Instructor.Create(account.Value.FullName, now).Value!;
        ownerInstructor.User.LinkAccount(userId.Value);
        instructorRepository.Add(ownerInstructor);
        businessMemberRepository.Add(BusinessMember.CreateBranchOwner(business.Id, ownerInstructor.User, ownerInstructor.Id));
        var signUpPlan = await subscriptionRepository.GetSignUpPlanAsync(cancellationToken);
        subscriptionRepository.Add(Subscription.StartAtListPrice(organization.Id, signUpPlan, now));
        if (pendingSignUp.LegalTermsAcceptanceOf(userId.Value) is { } legalTermsAcceptance)
        {
            legalTermsAcceptanceRepository.Add(legalTermsAcceptance);
        }

        pendingSignUp.Complete(now);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = await tokenService.IssueAsync(
            new SessionUser(userId.Value, account.Value.Email, business.Id, nameof(BusinessRole.BranchOwner)),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TokenResponse.From(tokens);
    }

    private static Result<TokenResponse> InvalidLink() =>
        Result.Validation<TokenResponse>(
            AuthenticationErrorCodes.InvalidSignUpTokenMessage,
            AuthenticationErrorCodes.InvalidSignUpToken);
}
