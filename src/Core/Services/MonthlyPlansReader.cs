using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Services;

public sealed class MonthlyPlansReader(
    IStudentPlanRepository studentPlanRepository,
    IClassPackRepository classPackRepository)
    : IMonthlyPlansReader
{
    private readonly Dictionary<string, IReadOnlyList<StudentPlanChange>> _planChangesByFamilies = [];
    private IReadOnlyDictionary<long, ClassPack>? _plansById;
    private IReadOnlyList<ClassPackPriceChange>? _priceChanges;

    public async Task<MonthlyPlans> ReadAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken)
    {
        var familiesKey = string.Join(',', clientIds.Distinct().Order());
        if (!_planChangesByFamilies.TryGetValue(familiesKey, out var planChanges))
        {
            planChanges = await studentPlanRepository.ListByClientsAsync([.. clientIds.Distinct()], cancellationToken);
            _planChangesByFamilies[familiesKey] = planChanges;
        }

        if (planChanges.Count == 0)
        {
            return new MonthlyPlans([], new Dictionary<long, ClassPack>(), []);
        }

        _plansById ??= (await classPackRepository.ListAsync(includeInactive: true, cancellationToken))
            .Where(classPack => classPack.RenewsMonthly)
            .ToDictionary(classPack => classPack.Id);
        _priceChanges ??= await classPackRepository.ListPriceChangesAsync(cancellationToken);
        return new MonthlyPlans(planChanges, _plansById, _priceChanges);
    }
}
