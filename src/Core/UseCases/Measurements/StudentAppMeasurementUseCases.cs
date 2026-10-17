using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Measurements;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Core.UseCases.Measurements;

public sealed record GetStudentAppMeasurementsQuery : IQuery;

public sealed record StudentAppStudentMeasurements(Guid StudentId, string FullName, IReadOnlyList<StudentMeasurementResponse> Measurements);

public sealed record StudentAppMeasurementsResponse(IReadOnlyList<StudentAppStudentMeasurements> Students);

public sealed record RecordStudentAppMeasurementCommand(Guid StudentId, Guid MeasurementId, DateOnly? MeasuredOn, decimal? Value, string? Note) : ICommand;

public sealed class GetStudentAppMeasurementsUseCase(
    IStudentAppAccess studentAppAccess,
    IStudentRepository studentRepository,
    IMeasurementDefinitionRepository definitionRepository,
    IMeasurementRecordRepository recordRepository,
    IMeasurementGoalRepository goalRepository,
    IUserRepository userRepository)
    : IUseCase<GetStudentAppMeasurementsQuery, StudentAppMeasurementsResponse>
{
    public async Task<Result<StudentAppMeasurementsResponse>> ExecuteAsync(GetStudentAppMeasurementsQuery command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var students = await studentRepository.ListByClientAsync(access.ClientId, cancellationToken);
        var studentIds = students.Select(student => student.Id).ToList();
        var definitions = (await definitionRepository.ListAsync(cancellationToken))
            .Where(definition => definition.IsVisibleToStudents)
            .ToList();
        var records = await recordRepository.ListByStudentsAsync(studentIds, cancellationToken);
        var goals = await goalRepository.ListByStudentsAsync(studentIds, cancellationToken);
        var recorderNames = await userRepository.ListFullNamesAsync(
            [.. records.Select(record => record.RecordedByUserId).OfType<Guid>().Distinct()], cancellationToken);

        return new StudentAppMeasurementsResponse(
        [
            .. students.Select(student => new StudentAppStudentMeasurements(
                student.Id,
                student.User.FullName,
                [
                    .. StudentMeasurements.For(student.Id, definitions, records, goals, recorderNames)
                        .Where(measurement => measurement.Records.Count > 0 || measurement.Measurement.IsRecordedByStudents),
                ])),
        ]);
    }
}

public sealed class RecordStudentAppMeasurementUseCase(
    IStudentAppAccess studentAppAccess,
    IStudentRepository studentRepository,
    IMeasurementDefinitionRepository definitionRepository,
    IMeasurementRecordRepository recordRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<RecordStudentAppMeasurementCommand, MeasurementRecordResponse>
{
    private const string NotRecordedByStudentsMessage = "Only the team records this measurement.";

    public async Task<Result<MeasurementRecordResponse>> ExecuteAsync(RecordStudentAppMeasurementCommand command, CancellationToken cancellationToken)
    {
        var ownStudentError = await AccountStudentRules.FindOwnStudentErrorAsync(studentAppAccess, studentRepository, command.StudentId, cancellationToken);
        if (ownStudentError is not null)
        {
            return ownStudentError;
        }

        var definition = await definitionRepository.GetAsync(command.MeasurementId, cancellationToken);
        if (definition is null || !definition.IsVisibleToStudents)
        {
            return MeasurementDefinitionFailures.NotFound();
        }

        if (!definition.IsRecordedByStudents)
        {
            return Result.Forbidden<MeasurementRecordResponse>(NotRecordedByStudentsMessage, MeasurementErrorCodes.NotRecordedByStudents);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var measuredOn = command.MeasuredOn ?? today;
        var existing = await recordRepository.FindOnDateForUpdateAsync(command.StudentId, definition.Id, measuredOn, cancellationToken);
        if (existing is { RecordedByStudent: false })
        {
            return MeasurementRecordFailures.SameDate();
        }

        var record = await MeasurementRecording.RecordAsync(
            recordRepository,
            definition,
            command.StudentId,
            measuredOn,
            command.Value,
            command.Note,
            today,
            timeProvider.GetUtcNow(),
            recordedByUserId: null,
            recordedByStudent: true,
            cancellationToken);
        if (record.IsFailure)
        {
            return record.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MeasurementRecordResponse.From(record.Value!);
    }
}
