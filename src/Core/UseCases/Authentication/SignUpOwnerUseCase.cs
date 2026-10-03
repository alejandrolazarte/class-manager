using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignUpOwnerCommand(
    string? OwnerFullName,
    string? Email,
    string? Password,
    string? BusinessName,
    string? TimeZoneId,
    string? CurrencyCode,
    string? DefaultCountryCallingCode);

public sealed class SignUpOwnerUseCase(
    IIdentityService identityService,
    ITokenService tokenService,
    IOrganizationRepository organizationRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IInstructorRepository instructorRepository,
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    ITenantScope tenantScope,
    TimeProvider timeProvider)
    : IUseCase<SignUpOwnerCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(SignUpOwnerCommand command, CancellationToken cancellationToken)
    {
        var account = OwnerAccount.Create(command.OwnerFullName, command.Email, command.Password);
        if (account.IsFailure)
        {
            return RenameField(account.Error!, nameof(OwnerAccount.FullName), nameof(SignUpOwnerCommand.OwnerFullName));
        }

        var now = timeProvider.GetUtcNow();
        var organization = Organization.Create(command.BusinessName, now);
        if (organization.IsFailure)
        {
            return RenameField(organization.Error!, nameof(Organization.Name), nameof(SignUpOwnerCommand.BusinessName));
        }

        var slug = await BusinessSlugAllocator.FindAvailableAsync(businessRepository, command.BusinessName, cancellationToken);
        var business = Business.Create(
            organization.Value!.Id,
            command.BusinessName,
            slug,
            command.TimeZoneId,
            command.CurrencyCode,
            command.DefaultCountryCallingCode,
            now);
        if (business.IsFailure)
        {
            return RenameField(business.Error!, nameof(Business.Name), nameof(SignUpOwnerCommand.BusinessName));
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

        tenantScope.Establish(business.Value!.Id);
        organizationRepository.Add(organization.Value);
        organizationMemberRepository.Add(OrganizationMember.CreateBrandOwner(organization.Value.Id, userId.Value));
        businessRepository.Add(business.Value);
        var ownerInstructor = Instructor.Create(account.Value.FullName).Value!;
        instructorRepository.Add(ownerInstructor);
        businessMemberRepository.Add(BusinessMember.CreateBranchOwner(business.Value.Id, userId.Value, ownerInstructor.Id));
        var signUpPlan = await subscriptionRepository.GetSignUpPlanAsync(cancellationToken);
        subscriptionRepository.Add(Subscription.StartAtListPrice(organization.Value.Id, signUpPlan, now));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = await tokenService.IssueAsync(
            new SessionUser(userId.Value, account.Value.Email, business.Value.Id, nameof(BusinessRole.BranchOwner)),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TokenResponse.From(tokens);
    }

    private static ResultError RenameField(ResultError error, string entityFieldName, string commandFieldName) =>
        error.FieldName == entityFieldName ? error with { FieldName = commandFieldName } : error;
}
