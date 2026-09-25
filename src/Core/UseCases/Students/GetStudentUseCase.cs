using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Students;

public sealed record GetStudentQuery(Guid StudentId);

public sealed class GetStudentUseCase(IStudentRepository studentRepository)
    : IUseCase<GetStudentQuery, StudentSummaryResponse>
{
    private const string NotFoundMessage = "The student does not exist.";

    public async Task<Result<StudentSummaryResponse>> ExecuteAsync(GetStudentQuery command, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetSummaryByIdAsync(command.StudentId, cancellationToken);
        if (student is null)
        {
            return Result.NotFound<StudentSummaryResponse>(NotFoundMessage, StudentErrorCodes.NotFound);
        }

        return StudentSummaryResponse.From(student);
    }
}
