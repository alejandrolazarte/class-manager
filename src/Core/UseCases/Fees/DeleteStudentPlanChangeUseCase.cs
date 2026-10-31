using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Fees;

public sealed record DeleteStudentPlanChangeCommand(long StudentId, [property: AuditPlain] string EffectiveFrom) : ICommand;

public sealed class DeleteStudentPlanChangeUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IStudentPlanRepository studentPlanRepository,
    IClassPackRepository classPackRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<DeleteStudentPlanChangeCommand, FamilyPlansResponse>
{
    public async Task<Result<FamilyPlansResponse>> ExecuteAsync(DeleteStudentPlanChangeCommand command, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetSummaryByIdAsync(command.StudentId, cancellationToken);
        if (student is null)
        {
            return StudentPlanFailures.StudentNotFound();
        }

        if (!await MoneyRules.CanCollectFromAsync(accessScopes, clientRepository, student.ClientId, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = UpcomingMonth.Resolve(command.EffectiveFrom, today, nameof(DeleteStudentPlanChangeCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = await studentPlanRepository.FindForUpdateAsync(student.Id, effectiveFrom.Value!.FirstDay, cancellationToken);
        if (change is null)
        {
            return StudentPlanFailures.ChangeNotFound();
        }

        studentPlanRepository.Remove(change);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await FamilyPlansReader.ReadAsync(student.ClientId, today, studentRepository, studentPlanRepository, classPackRepository, cancellationToken);
    }
}
