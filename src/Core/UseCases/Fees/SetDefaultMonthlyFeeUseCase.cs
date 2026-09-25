using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SetDefaultMonthlyFeeCommand(decimal? Amount);

public sealed class SetDefaultMonthlyFeeUseCase(IBusinessRepository businessRepository, IUnitOfWork unitOfWork)
    : IUseCase<SetDefaultMonthlyFeeCommand, BusinessResponse>
{
    public async Task<Result<BusinessResponse>> ExecuteAsync(SetDefaultMonthlyFeeCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BusinessResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var update = business.SetDefaultMonthlyFee(command.Amount);
        if (update.IsFailure)
        {
            return update.Error! with { FieldName = nameof(SetDefaultMonthlyFeeCommand.Amount) };
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BusinessResponse.From(business);
    }
}
