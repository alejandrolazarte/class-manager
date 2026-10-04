using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Achievements;

public sealed record GetAchievementSettingsQuery : IQuery;

public sealed record UpdateAchievementSettingsCommand(bool NoticedAbsencesKeepStreak, IReadOnlyList<LevelDefinition> Levels) : ICommand;

public sealed record AchievementSettingsResponse(bool NoticedAbsencesKeepStreak, IReadOnlyList<LevelDefinition> Levels);

public sealed class GetAchievementSettingsUseCase(
    IBusinessRepository businessRepository,
    IAchievementLevelRepository levelRepository)
    : IUseCase<GetAchievementSettingsQuery, AchievementSettingsResponse>
{
    public async Task<Result<AchievementSettingsResponse>> ExecuteAsync(GetAchievementSettingsQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<AchievementSettingsResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        return new AchievementSettingsResponse(business.NoticedAbsencesKeepStreak, await levelRepository.ListAsync(cancellationToken));
    }
}

public sealed class UpdateAchievementSettingsUseCase(
    IBusinessRepository businessRepository,
    IAchievementLevelRepository levelRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<UpdateAchievementSettingsCommand, AchievementSettingsResponse>
{
    public async Task<Result<AchievementSettingsResponse>> ExecuteAsync(UpdateAchievementSettingsCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<AchievementSettingsResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var levels = command.Levels ?? [];
        var error = LevelLadder.Validate(levels);
        if (error is not null)
        {
            return error;
        }

        IReadOnlyList<LevelDefinition> trimmedLevels = [.. levels.Select(level => level with { Name = level.Name.Trim() })];
        business.ChangeNoticedAbsencesKeepStreak(command.NoticedAbsencesKeepStreak);
        await levelRepository.ReplaceAsync(trimmedLevels, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AchievementSettingsResponse(business.NoticedAbsencesKeepStreak, trimmedLevels);
    }
}
