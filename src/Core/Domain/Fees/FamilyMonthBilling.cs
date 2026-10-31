namespace ClassManager.Core.Domain.Fees;

public sealed record StudentPlanLine(
    long StudentId,
    string StudentFullName,
    long ClassPackId,
    string PlanName,
    decimal Price,
    decimal? CustomPrice);

public sealed record FamilyMonthBilling(long ClientId, IReadOnlyList<StudentPlanLine> Lines)
{
    public bool HasMonthlyPlan => Lines.Count > 0;

    public decimal? Due => Lines.Sum(line => line.Price) is var total and > 0 ? total : null;
}
