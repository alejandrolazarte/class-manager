using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Families;

internal static class FamilyStudentRules
{
    private const string StudentNotFoundMessage = "The student is not part of this family.";

    public static async Task<ResultError?> FindOwnStudentErrorAsync(
        IFamilyAccess familyAccess,
        IStudentRepository studentRepository,
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var students = await studentRepository.ListByClientAsync(access.ClientId, cancellationToken);
        return students.Any(student => student.Id == studentId)
            ? null
            : new ResultError(StudentErrorCodes.NotFound, StudentNotFoundMessage, ErrorKind.NotFound);
    }
}
