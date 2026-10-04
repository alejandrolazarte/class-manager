using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.StudentApp;

internal static class AccountStudentRules
{
    private const string StudentNotFoundMessage = "The student is not part of this account.";

    public static async Task<ResultError?> FindOwnStudentErrorAsync(
        IStudentAppAccess studentAppAccess,
        IStudentRepository studentRepository,
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var students = await studentRepository.ListByClientAsync(access.ClientId, cancellationToken);
        return students.Any(student => student.Id == studentId)
            ? null
            : new ResultError(StudentErrorCodes.NotFound, StudentNotFoundMessage, ErrorKind.NotFound);
    }
}
