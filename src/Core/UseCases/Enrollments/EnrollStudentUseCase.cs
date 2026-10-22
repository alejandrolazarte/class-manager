using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Enrollments;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record EnrollStudentRequest(long? StudentId, DateOnly? StartDate, bool AllowOutsidePack = false)
{
    public EnrollStudentCommand ToCommand(long classGroupId) => new(classGroupId, StudentId, StartDate, AllowOutsidePack);
}

public sealed record EnrollStudentCommand(long ClassGroupId, long? StudentId, DateOnly? StartDate, bool AllowOutsidePack = false) : ICommand;

public sealed class EnrollStudentUseCase(
    IClassGroupRepository classGroupRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    IClientRepository clientRepository,
    IClassBalanceService classBalanceService,
    IClassPackRepository classPackRepository,
    IFeeScheduleRepository feeScheduleRepository)
    : IUseCase<EnrollStudentCommand, EnrollmentResponse>
{
    private const string ClassGroupNotFoundMessage = "The class group does not exist.";
    private const string StudentNotFoundMessage = "The student does not exist.";
    private const string ClassGroupInactiveMessage = "The class group is inactive.";
    private const string ClassGroupFullMessage = "The class group is full.";
    private const string AlreadyEnrolledMessage = "The student is already enrolled in this class group.";
    private const string OutsidePackMessage = "The student pays per class and none of their active class packs covers this class group.";

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
        if (existingEnrollment?.EndDate is not null)
        {
            return await ContinueAsync(existingEnrollment.Id, cancellationToken);
        }

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

        if (!command.AllowOutsidePack && await IsOutsideActivePacksAsync(student.ClientId, classGroup.Id, today, cancellationToken))
        {
            return Result.Conflict<EnrollmentResponse>(OutsidePackMessage, EnrollmentErrorCodes.OutsidePack);
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

    private async Task<bool> IsOutsideActivePacksAsync(long clientId, long classGroupId, DateOnly today, CancellationToken cancellationToken)
    {
        var plan = FeeTimeline.PlanIn(
            await feeScheduleRepository.ListClientPlanChangesAsync([clientId], cancellationToken),
            await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken),
            BillingMonth.From(today));
        if (!plan.PaysPerClass)
        {
            return false;
        }

        var balances = await classBalanceService.CalculateAsync([clientId], cancellationToken);
        var activePurchases = balances[clientId].Purchases
            .Where(usage => usage.Status == ClassPackPurchaseStatus.Active)
            .Select(usage => usage.Purchase)
            .ToList();
        foreach (var purchase in activePurchases)
        {
            if (await classPackRepository.GetByIdAsync(purchase.ClassPackId, cancellationToken) is { } classPack
                && classPack.CoversClassGroup(classGroupId))
            {
                return false;
            }
        }

        return activePurchases.Count > 0;
    }

    private async Task<Result<EnrollmentResponse>> ContinueAsync(long leavingEnrollmentId, CancellationToken cancellationToken)
    {
        var leavingEnrollment = await enrollmentRepository.GetForUpdateAsync(leavingEnrollmentId, cancellationToken);
        if (leavingEnrollment is null)
        {
            return AlreadyEnrolled(leavingEnrollmentId);
        }

        leavingEnrollment.Continue();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return EnrollmentResponse.From(leavingEnrollment);
    }

    private static Result<EnrollmentResponse> AlreadyEnrolled(long? existingEnrollmentId) =>
        Result.Conflict<EnrollmentResponse>(
            AlreadyEnrolledMessage,
            EnrollmentErrorCodes.AlreadyEnrolled,
            new Dictionary<string, object?> { [EnrollmentErrorCodes.ExistingEnrollmentIdDetail] = existingEnrollmentId });
}
