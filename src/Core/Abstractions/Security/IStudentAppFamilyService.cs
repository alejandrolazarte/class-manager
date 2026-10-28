using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.Abstractions.Security;

public sealed record StudentAppFamilyMember(Student Student, FamilyMemberView View)
{
    public bool ShowsPersonalData => View == FamilyMemberView.Everything;
}

public sealed record StudentAppFamily(
    StudentAppAccess Access,
    Business Business,
    DateOnly Today,
    Client Client,
    IReadOnlyList<StudentAppFamilyMember> Members,
    IReadOnlyCollection<long> HiddenMemberUserIds,
    bool CanBuy)
{
    public IReadOnlyList<Student> Students => [.. Members.Select(member => member.Student)];

    public IReadOnlyList<long> StudentIds => [.. Members.Select(member => member.Student.Id)];

    public IReadOnlyList<long> PersonalDataStudentIds =>
        [.. Members.Where(member => member.ShowsPersonalData).Select(member => member.Student.Id)];

    public StudentAppFamilyMember? Find(long studentId) => Members.FirstOrDefault(member => member.Student.Id == studentId);

    public bool SeesOrderPlacedBy(long? createdByUserId) =>
        Access.PaysForFamily
            ? createdByUserId is not { } creatorUserId || !HiddenMemberUserIds.Contains(creatorUserId)
            : createdByUserId is not null && createdByUserId == Access.UserId;

    public OrderSearchCriteria Orders(bool awaitingPickupOnly, int limit) =>
        Access.PaysForFamily
            ? new OrderSearchCriteria(Client.Id, awaitingPickupOnly, limit, HiddenCreatorUserIds: HiddenMemberUserIds)
            : new OrderSearchCriteria(Client.Id, awaitingPickupOnly, limit, CreatorUserId: Access.UserId);
}

public interface IStudentAppFamilyService
{
    Task<StudentAppFamily?> GetAsync(CancellationToken cancellationToken);
}
