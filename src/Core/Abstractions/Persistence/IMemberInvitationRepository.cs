using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMemberInvitationRepository
{
    void Add(MemberInvitation invitation);

    Task<MemberInvitation?> GetForUpdateAsync(Guid invitationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MemberInvitation>> ListPendingAsync(DateTimeOffset now, CancellationToken cancellationToken);

    Task<IReadOnlyList<MemberInvitation>> ListPendingByCustomRoleAsync(Guid customRoleId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<IReadOnlyList<MemberInvitation>> ListPendingForUpdateByEmailAsync(string email, DateTimeOffset now, CancellationToken cancellationToken);

    Task<IReadOnlyList<MemberInvitation>> ListPendingForUpdateByInstructorAsync(Guid instructorId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<MemberInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
}
