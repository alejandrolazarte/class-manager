using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Students;

public sealed record GetStudentQuery(Guid StudentId) : IQuery;

public sealed class GetStudentUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes)
    : IUseCase<GetStudentQuery, StudentSummaryResponse>
{
    private const string NotFoundMessage = "The student does not exist.";

    public async Task<Result<StudentSummaryResponse>> ExecuteAsync(GetStudentQuery command, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetSummaryByIdAsync(command.StudentId, cancellationToken);
        if (student is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [student.ClientId], cancellationToken))
        {
            return Result.NotFound<StudentSummaryResponse>(NotFoundMessage, StudentErrorCodes.NotFound);
        }

        return StudentSummaryResponse.From(student);
    }
}
