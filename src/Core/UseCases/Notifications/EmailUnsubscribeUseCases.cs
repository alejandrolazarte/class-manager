using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Notifications;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Notifications;

public sealed record GetEmailUnsubscriptionQuery([property: AuditSecret] string? Token) : IQuery;

public sealed record UnsubscribeFromEmailCommand([property: AuditSecret] string? Token) : ICommand;

public sealed record ResubscribeToEmailCommand([property: AuditSecret] string? Token) : ICommand;

public sealed record EmailUnsubscriptionResponse(string BusinessName, NotificationCategory Category, bool IsUnsubscribed);

public sealed class GetEmailUnsubscriptionUseCase(
    IEmailUnsubscribeTokenService tokens,
    ITenantScope tenantScope,
    IBusinessRepository businessRepository,
    INotificationOptOutRepository optOutRepository)
    : IUseCase<GetEmailUnsubscriptionQuery, EmailUnsubscriptionResponse>
{
    public async Task<Result<EmailUnsubscriptionResponse>> ExecuteAsync(GetEmailUnsubscriptionQuery query, CancellationToken cancellationToken)
    {
        var unsubscription = EmailUnsubscriptionLink.Open(tokens, tenantScope, query.Token);
        if (unsubscription is null)
        {
            return EmailUnsubscriptionLink.Invalid();
        }

        var optOut = await optOutRepository.FindAsync(
            unsubscription.AccountId, unsubscription.Category, NotificationChannel.Email, cancellationToken);
        return await EmailUnsubscriptionLink.RespondAsync(businessRepository, unsubscription, optOut is not null, cancellationToken);
    }
}

public sealed class UnsubscribeFromEmailUseCase(
    IEmailUnsubscribeTokenService tokens,
    ITenantScope tenantScope,
    IBusinessRepository businessRepository,
    INotificationOptOutRepository optOutRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<UnsubscribeFromEmailCommand, EmailUnsubscriptionResponse>
{
    public async Task<Result<EmailUnsubscriptionResponse>> ExecuteAsync(UnsubscribeFromEmailCommand command, CancellationToken cancellationToken)
    {
        var unsubscription = EmailUnsubscriptionLink.Open(tokens, tenantScope, command.Token);
        if (unsubscription is null)
        {
            return EmailUnsubscriptionLink.Invalid();
        }

        var optOut = await optOutRepository.FindAsync(
            unsubscription.AccountId, unsubscription.Category, NotificationChannel.Email, cancellationToken);
        if (optOut is null)
        {
            optOutRepository.Add(NotificationOptOut.Create(unsubscription.AccountId, unsubscription.Category, NotificationChannel.Email));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await EmailUnsubscriptionLink.RespondAsync(businessRepository, unsubscription, isUnsubscribed: true, cancellationToken);
    }
}

public sealed class ResubscribeToEmailUseCase(
    IEmailUnsubscribeTokenService tokens,
    ITenantScope tenantScope,
    IBusinessRepository businessRepository,
    INotificationOptOutRepository optOutRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<ResubscribeToEmailCommand, EmailUnsubscriptionResponse>
{
    public async Task<Result<EmailUnsubscriptionResponse>> ExecuteAsync(ResubscribeToEmailCommand command, CancellationToken cancellationToken)
    {
        var unsubscription = EmailUnsubscriptionLink.Open(tokens, tenantScope, command.Token);
        if (unsubscription is null)
        {
            return EmailUnsubscriptionLink.Invalid();
        }

        var optOut = await optOutRepository.FindAsync(
            unsubscription.AccountId, unsubscription.Category, NotificationChannel.Email, cancellationToken);
        if (optOut is not null)
        {
            optOutRepository.Remove(optOut);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await EmailUnsubscriptionLink.RespondAsync(businessRepository, unsubscription, isUnsubscribed: false, cancellationToken);
    }
}

internal static class EmailUnsubscriptionLink
{
    public static EmailUnsubscription? Open(IEmailUnsubscribeTokenService tokens, ITenantScope tenantScope, string? token)
    {
        var unsubscription = tokens.Read(token);
        if (unsubscription is null || !NotificationCategories.Of(unsubscription.Category).Uses(NotificationChannel.Email))
        {
            return null;
        }

        tenantScope.Establish(unsubscription.BusinessId);
        return unsubscription;
    }

    public static Result<EmailUnsubscriptionResponse> Invalid() =>
        Result.Validation<EmailUnsubscriptionResponse>(
            NotificationErrorCodes.InvalidUnsubscribeLinkMessage, NotificationErrorCodes.InvalidUnsubscribeLink);

    public static async Task<Result<EmailUnsubscriptionResponse>> RespondAsync(
        IBusinessRepository businessRepository, EmailUnsubscription unsubscription, bool isUnsubscribed, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        return business is null
            ? Invalid()
            : new EmailUnsubscriptionResponse(business.BrandDisplayName, unsubscription.Category, isUnsubscribed);
    }
}
