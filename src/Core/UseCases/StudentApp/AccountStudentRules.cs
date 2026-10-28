using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.StudentApp;

internal static class AccountStudentRules
{
    private const string StudentNotFoundMessage = "The student is not part of this account.";

    public static async Task<ResultError?> FindOwnStudentErrorAsync(
        IStudentAppFamilyService studentAppFamily,
        long studentId,
        CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        return family.Find(studentId) is null ? StudentNotFound() : null;
    }

    public static async Task<ResultError?> FindPersonalDataErrorAsync(
        IStudentAppFamilyService studentAppFamily,
        long studentId,
        CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        return family.Find(studentId) is { ShowsPersonalData: true } ? null : StudentNotFound();
    }

    private static ResultError StudentNotFound() =>
        new(StudentErrorCodes.NotFound, StudentNotFoundMessage, ErrorKind.NotFound);
}
