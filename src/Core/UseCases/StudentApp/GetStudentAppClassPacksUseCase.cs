using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppClassPacksQuery : IQuery;

public sealed record StudentAppPackClassResponse(DateOnly Date, string ClassName, long? StudentId, bool IsPrivateLesson)
{
    public static StudentAppPackClassResponse From(AttendedClass attendedClass) =>
        new(attendedClass.Date, attendedClass.ClassGroupName, attendedClass.StudentId, attendedClass.IsPrivateLesson);
}

public sealed record StudentAppPackPurchaseResponse(
    long Id,
    string Name,
    int ClassCount,
    DateOnly PurchasedOn,
    DateOnly? ExpiresOn,
    int UsedClasses,
    int RemainingClasses,
    ClassPackPurchaseStatus Status,
    bool CoversAnyGroupClass,
    bool IsForPrivateLessons,
    IReadOnlyList<string> ClassGroupNames,
    IReadOnlyList<StudentAppPackClassResponse> Classes);

public sealed record StudentAppClassPacksResponse(
    int AvailableClasses,
    IReadOnlyList<StudentAppPackClassResponse> UnpaidClasses,
    IReadOnlyList<StudentAppPackPurchaseResponse> Purchases);

public sealed class GetStudentAppClassPacksUseCase(
    IStudentAppFamilyService studentAppFamily,
    IClassBalanceService classBalanceService,
    IClassGroupRepository classGroupRepository)
    : IUseCase<GetStudentAppClassPacksQuery, StudentAppClassPacksResponse>
{
    public async Task<Result<StudentAppClassPacksResponse>> ExecuteAsync(GetStudentAppClassPacksQuery command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var clientId = family.Client.Id;
        var balance = (await classBalanceService.CalculateAsync([clientId], cancellationToken))[clientId];
        var familyStudentIds = family.StudentIds.ToHashSet();
        bool IsOfFamily(AttendedClass attendedClass) => attendedClass.StudentId is { } studentId && familyStudentIds.Contains(studentId);

        IReadOnlyCollection<long> classGroupIds =
        [
            .. balance.Purchases
                .SelectMany(usage => usage.Purchase.ClassGroups)
                .Select(classGroup => classGroup.ClassGroupId)
                .Distinct(),
        ];
        var classGroupNames = (await classGroupRepository.ListByIdsAsync(classGroupIds, cancellationToken))
            .ToDictionary(classGroup => classGroup.Id, classGroup => classGroup.Name);
        var paidClassesByPurchase = balance.PaidClasses
            .Where(paidClass => IsOfFamily(paidClass.AttendedClass))
            .ToLookup(paidClass => paidClass.Purchase, paidClass => paidClass.AttendedClass);

        return new StudentAppClassPacksResponse(
            balance.AvailableClasses,
            [.. balance.UnpaidAttendances.Where(IsOfFamily).OrderByDescending(attended => attended.Date).Select(StudentAppPackClassResponse.From)],
            [
                .. balance.Purchases.Select(usage => new StudentAppPackPurchaseResponse(
                    usage.Purchase.Id,
                    usage.Purchase.Name,
                    usage.Purchase.ClassCount,
                    usage.Purchase.PurchasedOn,
                    usage.Purchase.ExpiresOn,
                    usage.UsedClasses,
                    usage.RemainingClasses,
                    usage.Status,
                    usage.Purchase.CoversAnyGroupClass,
                    usage.Purchase.IsForPrivateLessons,
                    [
                        .. usage.Purchase.ClassGroups
                            .Select(classGroup => classGroupNames.GetValueOrDefault(classGroup.ClassGroupId))
                            .OfType<string>()
                            .Order(StringComparer.CurrentCulture),
                    ],
                    [.. paidClassesByPurchase[usage.Purchase].OrderByDescending(attended => attended.Date).Select(StudentAppPackClassResponse.From)])),
            ]);
    }
}
