using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
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
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IInstructorRepository instructorRepository,
    IUnitOfWork unitOfWork,
    ITenantScope tenantScope,
    TimeProvider timeProvider)
    : IUseCase<SignUpOwnerCommand, TokenResponse>
{
    private const int FirstSlugSuffix = 2;

    public async Task<Result<TokenResponse>> ExecuteAsync(SignUpOwnerCommand command, CancellationToken cancellationToken)
    {
        var account = OwnerAccount.Create(command.OwnerFullName, command.Email, command.Password);
        if (account.IsFailure)
        {
            return RenameField(account.Error!, nameof(OwnerAccount.FullName), nameof(SignUpOwnerCommand.OwnerFullName));
        }

        var slug = await FindAvailableSlugAsync(command.BusinessName, cancellationToken);
        var business = Business.Create(
            command.BusinessName,
            slug,
            command.TimeZoneId,
            command.CurrencyCode,
            command.DefaultCountryCallingCode,
            timeProvider.GetUtcNow());
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
        businessRepository.Add(business.Value);
        businessMemberRepository.Add(BusinessMember.CreateOwner(business.Value.Id, userId.Value));
        instructorRepository.Add(Instructor.Create(account.Value.FullName).Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = await tokenService.IssueAsync(
            new SessionUser(userId.Value, account.Value.Email, business.Value.Id, BusinessRole.Owner),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TokenResponse.From(tokens);
    }

    private async Task<string> FindAvailableSlugAsync(string? businessName, CancellationToken cancellationToken)
    {
        var baseSlug = BusinessSlug.FromName(businessName);
        var candidateSlug = baseSlug;

        for (var suffix = FirstSlugSuffix; await businessRepository.IsSlugTakenAsync(candidateSlug, cancellationToken); suffix++)
        {
            candidateSlug = BusinessSlug.WithSuffix(baseSlug, suffix);
        }

        return candidateSlug;
    }

    private static ResultError RenameField(ResultError error, string entityFieldName, string commandFieldName) =>
        error.FieldName == entityFieldName ? error with { FieldName = commandFieldName } : error;
}
