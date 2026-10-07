using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

internal static class InvitedPersonName
{
    public static async Task<string?> FindAsync(
        MemberInvitation invitation,
        IInstructorRepository instructorRepository,
        CancellationToken cancellationToken) =>
        invitation.InstructorId is { } instructorId
            ? (await instructorRepository.GetByIdAsync(instructorId, cancellationToken))?.FullName
            : null;
}
