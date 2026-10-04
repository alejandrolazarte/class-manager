using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record EnrollStudentRequest(Guid? StudentId, DateOnly? StartDate)
{
    public EnrollStudentCommand ToCommand(Guid classGroupId) => new(classGroupId, StudentId, StartDate);
}

public sealed record EnrollStudentCommand(Guid ClassGroupId, Guid? StudentId, DateOnly? StartDate) : ICommand;

public sealed class EnrollStudentUseCase(
    IClassGroupRepository classGroupRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    IClientRepository clientRepository)
    : IUseCase<EnrollStudentCommand, EnrollmentResponse>
{
    private const string ClassGroupNotFoundMessage = "The class group does not exist.";
    private const string StudentNotFoundMessage = "The student does not exist.";
    private const string ClassGroupInactiveMessage = "The class group is inactive.";
    private const string ClassGroupFullMessage = "The class group is full.";
    private const string AlreadyEnrolledMessage = "The student is already enrolled in this class group.";

    public async Task<Result<EnrollmentResponse>> ExecuteAsync(EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return Result.NotFound<EnrollmentResponse>(ClassGroupNotFoundMessage, ClassGroupErrorCodes.NotFound);
        }

        if (!classGroup.IsActive)
        {
            return Result.Conflict<EnrollmentResponse>(ClassGroupInactiveMessage, ClassGroupErrorCodes.Inactive);
        }

        var scope = await accessScopes.ForInstructorsAsync(Permissions.Enrollments.Manage, cancellationToken);
        if (!scope.Includes(classGroup.InstructorId))
        {
            return AccessRules.NotYours();
        }

        var student = command.StudentId is null ? null : await studentRepository.GetSummaryByIdAsync(command.StudentId.Value, cancellationToken);
        if (student is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [student.ClientId], cancellationToken))
        {
            return Result.NotFound<EnrollmentResponse>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var existingEnrollment = await enrollmentRepository.FindCurrentAsync(student.Id, classGroup.Id, today, cancellationToken);
        if (existingEnrollment is not null)
        {
            return AlreadyEnrolled(existingEnrollment.Id);
        }

        var enrolledCount = await enrollmentRepository.CountCurrentAsync(classGroup.Id, today, cancellationToken);
        if (enrolledCount >= classGroup.Capacity)
        {
            return Result.Conflict<EnrollmentResponse>(
                ClassGroupFullMessage,
                ClassGroupErrorCodes.Full,
                new Dictionary<string, object?> { [ClassGroupErrorCodes.CapacityDetail] = classGroup.Capacity });
        }

        var enrollment = Enrollment.Create(student.Id, classGroup.Id, command.StartDate ?? today, timeProvider.GetUtcNow());
        enrollmentRepository.Add(enrollment);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            var winnerEnrollment = await enrollmentRepository.FindCurrentAsync(student.Id, classGroup.Id, today, cancellationToken);
            return AlreadyEnrolled(winnerEnrollment?.Id);
        }

        return EnrollmentResponse.From(enrollment);
    }

    private static Result<EnrollmentResponse> AlreadyEnrolled(Guid? existingEnrollmentId) =>
        Result.Conflict<EnrollmentResponse>(
            AlreadyEnrolledMessage,
            EnrollmentErrorCodes.AlreadyEnrolled,
            new Dictionary<string, object?> { [EnrollmentErrorCodes.ExistingEnrollmentIdDetail] = existingEnrollmentId });
}
