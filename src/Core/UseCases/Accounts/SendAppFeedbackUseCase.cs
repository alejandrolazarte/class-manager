using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Emails;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record SendAppFeedbackCommand(
    [property: AuditPersonal] string? Message,
    [property: AuditPlain] string? Platform,
    [property: AuditPlain] string? AppVersion) : ICommand;

public sealed record SendAppFeedbackResponse(bool IsSent);

public sealed class SendAppFeedbackUseCase(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IUserRepository userRepository,
    IBusinessRepository businessRepository,
    ICurrentMember currentMember,
    IAppFeedbackInbox appFeedbackInbox,
    IEmailSender emailSender)
    : IUseCase<SendAppFeedbackCommand, SendAppFeedbackResponse>
{
    public const int MessageMaxLength = 4000;
    public const int AppDetailMaxLength = 40;
    public const string StudentAppRole = "StudentApp";
    public const string BrandOwnerRole = "BrandOwner";

    private const string MessageRequiredMessage = "Write a message of at most 4000 characters.";
    private const string AppDetailTooLongMessage = "Platform and app version must be at most 40 characters.";

    public async Task<Result<SendAppFeedbackResponse>> ExecuteAsync(SendAppFeedbackCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } userId)
        {
            return Result.Unauthorized<SendAppFeedbackResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var message = command.Message?.Trim() ?? string.Empty;
        if (message.Length == 0 || message.Length > MessageMaxLength)
        {
            return Result.Validation<SendAppFeedbackResponse>(MessageRequiredMessage, fieldName: nameof(SendAppFeedbackCommand.Message));
        }

        if (command.Platform?.Length > AppDetailMaxLength || command.AppVersion?.Length > AppDetailMaxLength)
        {
            return Result.Validation<SendAppFeedbackResponse>(AppDetailTooLongMessage);
        }

        var accounts = await identityService.ListAccountsAsync([userId], cancellationToken);
        var user = await userRepository.FindByAccountAsync(userId, cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (accounts.Count == 0 || user is null || business is null)
        {
            return Result.Unauthorized<SendAppFeedbackResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var sender = accounts[0];
        var details = new AppFeedbackEmailDetails(
            message,
            user.FullName,
            sender.Email,
            business.Name,
            RoleOf(await currentMember.GetAccessAsync(cancellationToken)),
            command.Platform?.Trim(),
            command.AppVersion?.Trim());
        var appFeedback = EmailTexts.Spanish.AppFeedback(details);
        await emailSender.SendAsync(
            new EmailMessage(appFeedbackInbox.Address, appFeedback.Subject, appFeedback.Content, ReplyTo: sender.Email),
            cancellationToken);

        return new SendAppFeedbackResponse(IsSent: true);
    }

    private static string RoleOf(MemberAccess? access) => access switch
    {
        null => StudentAppRole,
        { IsBrandOwner: true } => BrandOwnerRole,
        { BranchRole: { } branchRole } => branchRole.ToString(),
        _ => BrandOwnerRole,
    };
}
