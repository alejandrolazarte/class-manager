using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Measurements;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Measurements;

public sealed record GetStudentMeasurementsQuery(Guid StudentId) : IQuery;

public sealed record RecordMeasurementRequest(DateOnly? MeasuredOn, decimal? Value, string? Note)
{
    public RecordMeasurementCommand ToCommand(Guid studentId, Guid measurementId) => new(studentId, measurementId, MeasuredOn, Value, Note);
}

public sealed record RecordMeasurementCommand(Guid StudentId, Guid MeasurementId, DateOnly? MeasuredOn, decimal? Value, string? Note) : ICommand;

public sealed record ClassMeasurementEntry(Guid StudentId, decimal? Value);

public sealed record RecordClassMeasurementsCommand(Guid MeasurementId, DateOnly? MeasuredOn, IReadOnlyList<ClassMeasurementEntry>? Entries) : ICommand;

public sealed record DeleteMeasurementRecordCommand(Guid RecordId) : ICommand;

public sealed record RestoreMeasurementRecordCommand(Guid RecordId) : ICommand;

public sealed record SetMeasurementGoalRequest(decimal? Value)
{
    public SetMeasurementGoalCommand ToCommand(Guid studentId, Guid measurementId) => new(studentId, measurementId, Value);
}

public sealed record SetMeasurementGoalCommand(Guid StudentId, Guid MeasurementId, decimal? Value) : ICommand;

internal static class MeasurementRecordFailures
{
    private const string StudentNotFoundMessage = "The student doesn't exist.";
    private const string RecordNotFoundMessage = "The measurement record doesn't exist.";
    private const string NoEntriesMessage = "Write at least one value.";
    private const string SameDateMessage = "There is already a value for that date.";
    private const string RepeatedStudentMessage = "Each student can have only one value.";

    public static ResultError StudentNotFound() => new(StudentErrorCodes.NotFound, StudentNotFoundMessage, ErrorKind.NotFound);

    public static ResultError RecordNotFound() => new(MeasurementErrorCodes.RecordNotFound, RecordNotFoundMessage, ErrorKind.NotFound);

    public static ResultError NoEntries() => new(Result.ValidationCode, NoEntriesMessage, ErrorKind.Validation) { FieldName = nameof(RecordClassMeasurementsCommand.Entries) };

    public static ResultError RepeatedStudent() => new(Result.ValidationCode, RepeatedStudentMessage, ErrorKind.Validation) { FieldName = nameof(RecordClassMeasurementsCommand.Entries) };

    public static ResultError SameDate() => new(Result.ConflictCode, SameDateMessage, ErrorKind.Conflict);
}

internal sealed class MeasurementStudentAccess(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes)
{
    public async Task<bool> CanReachAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken)
    {
        var students = await studentRepository.ListSummariesByIdsAsync(studentIds, cancellationToken);
        return students.Count == studentIds.Distinct().Count()
            && await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, students.Select(student => student.ClientId), cancellationToken);
    }
}

internal static class MeasurementRecording
{
    public static async Task<Result<MeasurementRecord>> RecordAsync(
        IMeasurementRecordRepository recordRepository,
        MeasurementDefinition definition,
        Guid studentId,
        DateOnly measuredOn,
        decimal? value,
        string? note,
        DateOnly today,
        DateTimeOffset now,
        Guid? recordedByUserId,
        bool recordedByStudent,
        CancellationToken cancellationToken)
    {
        var existing = await recordRepository.FindOnDateForUpdateAsync(studentId, definition.Id, measuredOn, cancellationToken);
        if (existing is not null)
        {
            var replaced = existing.Replace(definition, value, note, recordedByUserId, recordedByStudent);
            return replaced.IsFailure ? replaced.Error! : existing;
        }

        var record = MeasurementRecord.Create(studentId, definition, measuredOn, value, note, today, now, recordedByUserId, recordedByStudent);
        if (record.IsSuccess)
        {
            recordRepository.Add(record.Value!);
        }

        return record;
    }
}

public sealed class GetStudentMeasurementsUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes,
    IMeasurementDefinitionRepository definitionRepository,
    IMeasurementRecordRepository recordRepository,
    IMeasurementGoalRepository goalRepository,
    IUserRepository userRepository)
    : IUseCase<GetStudentMeasurementsQuery, IReadOnlyList<StudentMeasurementResponse>>
{
    public async Task<Result<IReadOnlyList<StudentMeasurementResponse>>> ExecuteAsync(GetStudentMeasurementsQuery command, CancellationToken cancellationToken)
    {
        if (!await new MeasurementStudentAccess(studentRepository, clientRepository, accessScopes).CanReachAsync([command.StudentId], cancellationToken))
        {
            return MeasurementRecordFailures.StudentNotFound();
        }

        var definitions = await definitionRepository.ListAsync(cancellationToken);
        var records = await recordRepository.ListByStudentsAsync([command.StudentId], cancellationToken);
        var goals = await goalRepository.ListByStudentsAsync([command.StudentId], cancellationToken);
        var recorderNames = await userRepository.ListFullNamesAsync(
            [.. records.Select(record => record.RecordedByUserId).OfType<Guid>().Distinct()], cancellationToken);

        return Result.Success(StudentMeasurements.For(command.StudentId, definitions, records, goals, recorderNames));
    }
}

public sealed class RecordMeasurementUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes,
    IMeasurementDefinitionRepository definitionRepository,
    IMeasurementRecordRepository recordRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    ICurrentMember currentMember)
    : IUseCase<RecordMeasurementCommand, MeasurementRecordResponse>
{
    public async Task<Result<MeasurementRecordResponse>> ExecuteAsync(RecordMeasurementCommand command, CancellationToken cancellationToken)
    {
        var definition = await definitionRepository.GetAsync(command.MeasurementId, cancellationToken);
        if (definition is null)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        if (!await new MeasurementStudentAccess(studentRepository, clientRepository, accessScopes).CanReachAsync([command.StudentId], cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var record = await MeasurementRecording.RecordAsync(
            recordRepository,
            definition,
            command.StudentId,
            command.MeasuredOn ?? today,
            command.Value,
            command.Note,
            today,
            timeProvider.GetUtcNow(),
            access?.UserId,
            recordedByStudent: false,
            cancellationToken);
        if (record.IsFailure)
        {
            return record.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MeasurementRecordResponse.From(record.Value!);
    }
}

public sealed class RecordClassMeasurementsUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes,
    IMeasurementDefinitionRepository definitionRepository,
    IMeasurementRecordRepository recordRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    ICurrentMember currentMember)
    : IUseCase<RecordClassMeasurementsCommand, IReadOnlyList<MeasurementRecordResponse>>
{
    public async Task<Result<IReadOnlyList<MeasurementRecordResponse>>> ExecuteAsync(RecordClassMeasurementsCommand command, CancellationToken cancellationToken)
    {
        var entries = command.Entries ?? [];
        if (entries.Count == 0)
        {
            return MeasurementRecordFailures.NoEntries();
        }

        var definition = await definitionRepository.GetAsync(command.MeasurementId, cancellationToken);
        if (definition is null)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        var studentIds = entries.Select(entry => entry.StudentId).Distinct().ToList();
        if (studentIds.Count != entries.Count)
        {
            return MeasurementRecordFailures.RepeatedStudent();
        }

        if (!await new MeasurementStudentAccess(studentRepository, clientRepository, accessScopes).CanReachAsync(studentIds, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var recorded = new List<MeasurementRecord>();
        foreach (var entry in entries)
        {
            var record = await MeasurementRecording.RecordAsync(
                recordRepository, definition, entry.StudentId, command.MeasuredOn ?? today, entry.Value, null, today, now, access?.UserId, recordedByStudent: false, cancellationToken);
            if (record.IsFailure)
            {
                return record.Error!;
            }

            recorded.Add(record.Value!);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<IReadOnlyList<MeasurementRecordResponse>>([.. recorded.Select(record => MeasurementRecordResponse.From(record))]);
    }
}

public sealed class DeleteMeasurementRecordUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes,
    IMeasurementRecordRepository recordRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<DeleteMeasurementRecordCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(DeleteMeasurementRecordCommand command, CancellationToken cancellationToken)
    {
        var record = await recordRepository.FindForUpdateAsync(command.RecordId, cancellationToken);
        if (record is null)
        {
            return MeasurementRecordFailures.RecordNotFound();
        }

        if (!await new MeasurementStudentAccess(studentRepository, clientRepository, accessScopes).CanReachAsync([record.StudentId], cancellationToken))
        {
            return AccessRules.NotYours();
        }

        recordRepository.Remove(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class RestoreMeasurementRecordUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes,
    IMeasurementRecordRepository recordRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<RestoreMeasurementRecordCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RestoreMeasurementRecordCommand command, CancellationToken cancellationToken)
    {
        var record = await recordRepository.FindDeletedForUpdateAsync(command.RecordId, cancellationToken);
        if (record is null)
        {
            return MeasurementRecordFailures.RecordNotFound();
        }

        if (!await new MeasurementStudentAccess(studentRepository, clientRepository, accessScopes).CanReachAsync([record.StudentId], cancellationToken))
        {
            return AccessRules.NotYours();
        }

        if (await recordRepository.FindOnDateForUpdateAsync(record.StudentId, record.MeasurementDefinitionId, record.MeasuredOn, cancellationToken) is not null)
        {
            return MeasurementRecordFailures.SameDate();
        }

        record.Restore();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class SetMeasurementGoalUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes,
    IMeasurementDefinitionRepository definitionRepository,
    IMeasurementGoalRepository goalRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<SetMeasurementGoalCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(SetMeasurementGoalCommand command, CancellationToken cancellationToken)
    {
        var definition = await definitionRepository.GetAsync(command.MeasurementId, cancellationToken);
        if (definition is null)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        if (!await new MeasurementStudentAccess(studentRepository, clientRepository, accessScopes).CanReachAsync([command.StudentId], cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var goal = await goalRepository.FindForUpdateAsync(command.StudentId, definition.Id, cancellationToken);
        if (command.Value is null)
        {
            if (goal is not null)
            {
                goalRepository.Remove(goal);
            }
        }
        else if (goal is null)
        {
            var created = MeasurementGoal.Create(command.StudentId, definition, command.Value);
            if (created.IsFailure)
            {
                return created.Error!;
            }

            goalRepository.Add(created.Value!);
        }
        else
        {
            var changed = goal.Change(definition, command.Value);
            if (changed.IsFailure)
            {
                return changed.Error!;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
