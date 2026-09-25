using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Students;

public sealed record SearchStudentsQuery(string? Search, int? Limit);

public sealed class SearchStudentsUseCase(
    IBusinessRepository businessRepository,
    IStudentRepository studentRepository)
    : IUseCase<SearchStudentsQuery, IReadOnlyList<StudentSummaryResponse>>
{
    public const int DefaultLimit = 20;
    public const int MaximumLimit = 50;
    public const int MinimumLimit = 1;

    public async Task<Result<IReadOnlyList<StudentSummaryResponse>>> ExecuteAsync(SearchStudentsQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<IReadOnlyList<StudentSummaryResponse>>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var search = string.IsNullOrWhiteSpace(command.Search) ? null : command.Search.Trim();
        var criteria = new StudentSearchCriteria(
            search,
            PhoneNumber.ToNormalizedPrefix(search, business.DefaultCountryCallingCode),
            Math.Clamp(command.Limit ?? DefaultLimit, MinimumLimit, MaximumLimit));

        var students = await studentRepository.SearchAsync(criteria, cancellationToken);

        return Result.Success<IReadOnlyList<StudentSummaryResponse>>([.. students.Select(StudentSummaryResponse.From)]);
    }
}
