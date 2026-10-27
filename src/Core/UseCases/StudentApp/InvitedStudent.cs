using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.StudentApp;

internal sealed record InvitedStudent(string? FullName, bool AsksBirthDate)
{
    private static readonly InvitedStudent Unknown = new(null, false);

    public static async Task<InvitedStudent> FindAsync(
        ClientInvitation invitation,
        IClientRepository clientRepository,
        IStudentRepository studentRepository,
        CancellationToken cancellationToken)
    {
        if (invitation.StudentId is { } studentId)
        {
            var student = await studentRepository.GetSummaryByIdAsync(studentId, cancellationToken);
            return student is null ? Unknown : new InvitedStudent(student.FullName, false);
        }

        var client = await clientRepository.GetByIdAsync(invitation.ClientId, cancellationToken);
        return new InvitedStudent(client?.User.FullName, IsBirthDateAsked(invitation, client?.User.BirthDate));
    }

    public static bool IsBirthDateAsked(ClientInvitation invitation, DateOnly? birthDateOnFile) =>
        invitation.StudentId is null && birthDateOnFile is null;
}
