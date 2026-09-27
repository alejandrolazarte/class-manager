using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record UpdateClassPackRequest(string? Name, int? ClassCount, decimal? Price, int? ValidityMonths)
{
    public UpdateClassPackCommand ToCommand(Guid classPackId) => new(classPackId, Name, ClassCount, Price, ValidityMonths);
}

public sealed record UpdateClassPackCommand(Guid ClassPackId, string? Name, int? ClassCount, decimal? Price, int? ValidityMonths);

public sealed class UpdateClassPackUseCase(IClassPackRepository classPackRepository, IUnitOfWork unitOfWork)
    : IUseCase<UpdateClassPackCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(UpdateClassPackCommand command, CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetForUpdateAsync(command.ClassPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        var update = classPack.Update(command.Name, command.ClassCount, command.Price, command.ValidityMonths);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        var packWithSameName = await classPackRepository.FindByNameAsync(classPack.Name, cancellationToken);
        if (packWithSameName is not null && packWithSameName.Id != classPack.Id)
        {
            return ClassPackFailures.NameTaken();
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return ClassPackFailures.NameTaken();
        }

        return ClassPackResponse.From(classPack);
    }
}
