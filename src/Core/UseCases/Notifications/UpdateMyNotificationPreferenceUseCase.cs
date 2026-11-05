using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.UseCases.Notifications;

public sealed record UpdateMyNotificationPreferenceCommand(NotificationCategory Category, NotificationChannel Channel, bool IsEnabled) : ICommand;

public sealed class UpdateMyNotificationPreferenceUseCase(
    ICurrentUser currentUser,
    INotificationOptOutRepository optOutRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<UpdateMyNotificationPreferenceCommand, NotificationPreferenceResponse>
{
    public async Task<Result<NotificationPreferenceResponse>> ExecuteAsync(
        UpdateMyNotificationPreferenceCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return Result.Unauthorized<NotificationPreferenceResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var definition = NotificationCategories.Of(command.Category);
        if (!definition.Uses(command.Channel))
        {
            return Result.Validation<NotificationPreferenceResponse>(
                NotificationErrorCodes.ChannelNotInCategoryMessage,
                NotificationErrorCodes.ChannelNotInCategory,
                nameof(UpdateMyNotificationPreferenceCommand.Channel));
        }

        var optOut = await optOutRepository.FindAsync(accountId, command.Category, command.Channel, cancellationToken);
        if (command.IsEnabled && optOut is not null)
        {
            optOutRepository.Remove(optOut);
        }
        else if (!command.IsEnabled && optOut is null)
        {
            optOutRepository.Add(NotificationOptOut.Create(accountId, command.Category, command.Channel));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return NotificationPreferenceResponse.From(definition, await optOutRepository.ListAsync(accountId, cancellationToken));
    }
}
