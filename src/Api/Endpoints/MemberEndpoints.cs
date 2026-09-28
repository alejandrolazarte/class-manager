using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.Endpoints;

internal static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Me, GetCurrentMemberAsync).RequireMember();

        return endpoints;
    }

    private static async Task<IResult> GetCurrentMemberAsync(
        IUseCase<GetCurrentMemberQuery, CurrentMemberResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetCurrentMemberQuery(), cancellationToken);

        return result.ToOkResult();
    }
}
