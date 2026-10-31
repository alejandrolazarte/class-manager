using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.UseCases.Fees;

internal static class FamilyPlansReader
{
    public static async Task<FamilyPlansResponse> ReadAsync(
        long clientId,
        DateOnly today,
        IStudentRepository studentRepository,
        IStudentPlanRepository studentPlanRepository,
        IClassPackRepository classPackRepository,
        CancellationToken cancellationToken) =>
        FamilyPlansResponse.From(
            clientId,
            await studentRepository.ListByClientAsync(clientId, cancellationToken),
            await studentPlanRepository.ListByClientsAsync([clientId], cancellationToken),
            await classPackRepository.ListAsync(includeInactive: true, cancellationToken),
            await classPackRepository.ListPriceChangesAsync(cancellationToken),
            today);
}
