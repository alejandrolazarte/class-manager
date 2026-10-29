using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record ListInstructorsQuery(bool IncludeInactive) : IQuery;

public sealed class ListInstructorsUseCase(IInstructorRepository instructorRepository,
    IUserRepository userRepository,
    IUserPhotoUrlService userPhotoUrls)
    : IUseCase<ListInstructorsQuery, IReadOnlyList<InstructorResponse>>
{
    public async Task<Result<IReadOnlyList<InstructorResponse>>> ExecuteAsync(ListInstructorsQuery command, CancellationToken cancellationToken)
    {
        var instructors = command.IncludeInactive
            ? await instructorRepository.ListAllAsync(cancellationToken)
            : await instructorRepository.ListActiveAsync(cancellationToken);

        var clientIdsByUser = await userRepository.ListClientIdsByUserAsync([.. instructors.Select(instructor => instructor.UserId)], cancellationToken);
        var photoUrls = await userPhotoUrls.OfInstructorsAsync(instructors.Select(instructor => (long?)instructor.Id), cancellationToken);
        return Result.Success<IReadOnlyList<InstructorResponse>>([
            .. instructors.Select(instructor =>
                InstructorResponse.From(instructor) with
                {
                    SamePersonClientId = clientIdsByUser.TryGetValue(instructor.UserId, out var clientId) ? clientId : null,
                    PhotoUrl = photoUrls.GetValueOrDefault(instructor.Id),
                }),
        ]);
    }
}
