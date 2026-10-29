using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppFamilyQuery : IQuery;

public sealed record StudentAppFamilyMemberResponse(string FullName, string? Email, bool PaysForFamily, bool IsSignedIn, string? PhotoUrl = null);

public sealed record StudentAppFamilyResponse(IReadOnlyList<StudentAppFamilyMemberResponse> Members);

public sealed class GetStudentAppFamilyUseCase(IStudentAppFamilyService studentAppFamily, IUserPhotoUrlService userPhotoUrls)
    : IUseCase<GetStudentAppFamilyQuery, StudentAppFamilyResponse>
{
    public async Task<Result<StudentAppFamilyResponse>> ExecuteAsync(GetStudentAppFamilyQuery command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var payer = family.Client.User;
        var photoUrls = await userPhotoUrls.OfUsersAsync([payer.Id, .. family.EveryStudent.Select(student => student.UserId)], cancellationToken);
        return new StudentAppFamilyResponse(
        [
            new StudentAppFamilyMemberResponse(payer.FullName, payer.Email, PaysForFamily: true, IsSignedIn: family.Access.PaysForFamily, photoUrls.GetValueOrDefault(payer.Id)),
            .. family.EveryStudent
                .Where(student => student.UserId != payer.Id)
                .OrderBy(student => student.CreatedAt)
                .ThenBy(student => student.User.FullName, StringComparer.CurrentCultureIgnoreCase)
                .Select(student => new StudentAppFamilyMemberResponse(
                    student.User.FullName,
                    student.User.Email,
                    PaysForFamily: false,
                    IsSignedIn: student.Id == family.Access.StudentId,
                    photoUrls.GetValueOrDefault(student.UserId))),
        ]);
    }
}
