using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.UseCases.Measurements;

public sealed record ListMeasurementsQuery : IQuery;

public sealed record CreateMeasurementCommand(
    [property: AuditPlain] string? Name,
    MeasurementKind? Kind,
    [property: AuditPlain] string? Unit,
    ImprovementDirection? ImprovesWhen,
    bool IsVisibleToStudents,
    bool IsRecordedByStudents) : ICommand;

public sealed record UpdateMeasurementRequest(string? Name, ImprovementDirection? ImprovesWhen, bool IsVisibleToStudents, bool IsRecordedByStudents)
{
    public UpdateMeasurementCommand ToCommand(long measurementId) =>
        new(measurementId, Name, ImprovesWhen, IsVisibleToStudents, IsRecordedByStudents);
}

public sealed record UpdateMeasurementCommand(
    long MeasurementId,
    [property: AuditPlain] string? Name,
    ImprovementDirection? ImprovesWhen,
    bool IsVisibleToStudents,
    bool IsRecordedByStudents) : ICommand;

public sealed record DeleteMeasurementCommand(long MeasurementId) : ICommand;

public sealed record RestoreMeasurementCommand(long MeasurementId) : ICommand;

internal static class MeasurementDefinitionFailures
{
    private const string NotFoundMessage = "The measurement doesn't exist.";
    private const string DuplicateNameMessage = "There is already a measurement with that name.";

    public static ResultError NotFound() => new(MeasurementErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError DuplicateName() =>
        new(MeasurementErrorCodes.DuplicateName, DuplicateNameMessage, ErrorKind.Conflict) { FieldName = nameof(MeasurementDefinition.Name) };
}

public sealed class ListMeasurementsUseCase(IMeasurementDefinitionRepository definitionRepository)
    : IUseCase<ListMeasurementsQuery, IReadOnlyList<MeasurementResponse>>
{
    public async Task<Result<IReadOnlyList<MeasurementResponse>>> ExecuteAsync(ListMeasurementsQuery command, CancellationToken cancellationToken)
    {
        var definitions = await definitionRepository.ListAsync(cancellationToken);
        return Result.Success<IReadOnlyList<MeasurementResponse>>([.. definitions.Select(MeasurementResponse.From)]);
    }
}

public sealed class CreateMeasurementUseCase(
    IMeasurementDefinitionRepository definitionRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CreateMeasurementCommand, MeasurementResponse>
{
    public async Task<Result<MeasurementResponse>> ExecuteAsync(CreateMeasurementCommand command, CancellationToken cancellationToken)
    {
        var definition = MeasurementDefinition.Create(
            command.Name,
            command.Kind,
            command.Unit,
            command.ImprovesWhen,
            command.IsVisibleToStudents,
            command.IsRecordedByStudents,
            timeProvider.GetUtcNow());
        if (definition.IsFailure)
        {
            return definition.Error!;
        }

        if (await definitionRepository.NameExistsAsync(definition.Value!.Name, null, cancellationToken))
        {
            return MeasurementDefinitionFailures.DuplicateName();
        }

        definitionRepository.Add(definition.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MeasurementResponse.From(definition.Value);
    }
}

public sealed class UpdateMeasurementUseCase(IMeasurementDefinitionRepository definitionRepository, IUnitOfWork unitOfWork)
    : IUseCase<UpdateMeasurementCommand, MeasurementResponse>
{
    public async Task<Result<MeasurementResponse>> ExecuteAsync(UpdateMeasurementCommand command, CancellationToken cancellationToken)
    {
        var definition = await definitionRepository.FindForUpdateAsync(command.MeasurementId, cancellationToken);
        if (definition is null)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        var updated = definition.Update(command.Name, command.ImprovesWhen, command.IsVisibleToStudents, command.IsRecordedByStudents);
        if (updated.IsFailure)
        {
            return updated.Error!;
        }

        if (await definitionRepository.NameExistsAsync(definition.Name, definition.Id, cancellationToken))
        {
            return MeasurementDefinitionFailures.DuplicateName();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MeasurementResponse.From(definition);
    }
}

public sealed class DeleteMeasurementUseCase(IMeasurementDefinitionRepository definitionRepository, IUnitOfWork unitOfWork)
    : IUseCase<DeleteMeasurementCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(DeleteMeasurementCommand command, CancellationToken cancellationToken)
    {
        var definition = await definitionRepository.FindForUpdateAsync(command.MeasurementId, cancellationToken);
        if (definition is null)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        definitionRepository.Remove(definition);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class RestoreMeasurementUseCase(IMeasurementDefinitionRepository definitionRepository, IUnitOfWork unitOfWork)
    : IUseCase<RestoreMeasurementCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RestoreMeasurementCommand command, CancellationToken cancellationToken)
    {
        var definition = await definitionRepository.FindDeletedForUpdateAsync(command.MeasurementId, cancellationToken);
        if (definition is null)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        if (await definitionRepository.NameExistsAsync(definition.Name, definition.Id, cancellationToken))
        {
            return MeasurementDefinitionFailures.DuplicateName();
        }

        definition.Restore();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
