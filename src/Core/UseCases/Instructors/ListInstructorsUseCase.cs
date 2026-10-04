using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record ListInstructorsQuery(bool IncludeInactive) : IQuery;

public sealed class ListInstructorsUseCase(IInstructorRepository instructorRepository)
    : IUseCase<ListInstructorsQuery, IReadOnlyList<InstructorResponse>>
{
    public async Task<Result<IReadOnlyList<InstructorResponse>>> ExecuteAsync(ListInstructorsQuery command, CancellationToken cancellationToken)
    {
        var instructors = command.IncludeInactive
            ? await instructorRepository.ListAllAsync(cancellationToken)
            : await instructorRepository.ListActiveAsync(cancellationToken);

        return Result.Success<IReadOnlyList<InstructorResponse>>([.. instructors.Select(InstructorResponse.From)]);
    }
}
