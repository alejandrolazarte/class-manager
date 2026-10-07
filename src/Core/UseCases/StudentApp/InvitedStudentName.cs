using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.StudentApp;

internal static class InvitedStudentName
{
    public static async Task<string?> FindAsync(
        ClientInvitation invitation,
        IClientRepository clientRepository,
        IStudentRepository studentRepository,
        CancellationToken cancellationToken) =>
        invitation.StudentId is { } studentId
            ? (await studentRepository.GetSummaryByIdAsync(studentId, cancellationToken))?.FullName
            : (await clientRepository.GetByIdAsync(invitation.ClientId, cancellationToken))?.FullName;
}
