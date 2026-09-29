using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Branches;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.Endpoints;

internal static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Me, GetCurrentMemberAsync).RequireMember();
        endpoints.MapGet(ApiRoutes.MyBranches, ListBranchesAsync).RequireMember();
        endpoints.MapPost(ApiRoutes.OrganizationBranches, CreateBranchAsync).RequirePermission(Permissions.Brand.CreateBranches);

        var members = endpoints.MapGroup(ApiRoutes.Members);
        members.MapGet("/", GetTeamAsync).RequirePermission(Permissions.Members.View);
        members.MapPost(ApiRoutes.InvitationsSegment, InviteAsync).RequirePermission(Permissions.Members.Manage);
        members.MapDelete(ApiRoutes.InvitationById, RevokeInvitationAsync).RequirePermission(Permissions.Members.Manage);
        members.MapPost(ApiRoutes.InvitationById + ApiRoutes.Resend, ResendInvitationAsync).RequirePermission(Permissions.Members.Manage);
        members.MapPut(ApiRoutes.MemberById, ChangeRoleAsync).RequirePermission(Permissions.Members.Manage);
        members.MapDelete(ApiRoutes.MemberById, RemoveAsync).RequirePermission(Permissions.Members.Manage);
        members.MapPut(ApiRoutes.MemberById + ApiRoutes.BrandOwnerSegment, MakeBrandOwnerAsync)
            .RequirePermission(Permissions.Brand.ManageBrandOwners);
        members.MapDelete(ApiRoutes.MemberById + ApiRoutes.BrandOwnerSegment, RemoveBrandOwnerAsync)
            .RequirePermission(Permissions.Brand.ManageBrandOwners);

        return endpoints;
    }

    private static async Task<IResult> GetCurrentMemberAsync(
        IUseCase<GetCurrentMemberQuery, CurrentMemberResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetCurrentMemberQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListBranchesAsync(
        IUseCase<ListBranchesQuery, IReadOnlyList<BranchResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListBranchesQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateBranchAsync(
        CreateBranchCommand command,
        IUseCase<CreateBranchCommand, BranchResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(branch => TypedResults.Created(ApiRoutes.MyBranches, branch));
    }

    private static async Task<IResult> MakeBrandOwnerAsync(
        Guid memberId,
        IUseCase<SetBrandOwnerCommand, BrandOwnerResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetBrandOwnerCommand(memberId, IsBrandOwner: true), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RemoveBrandOwnerAsync(
        Guid memberId,
        IUseCase<SetBrandOwnerCommand, BrandOwnerResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetBrandOwnerCommand(memberId, IsBrandOwner: false), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ResendInvitationAsync(
        Guid invitationId,
        IUseCase<ResendInvitationCommand, InvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ResendInvitationCommand(invitationId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetTeamAsync(
        IUseCase<GetTeamQuery, TeamResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetTeamQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> InviteAsync(
        InviteMemberCommand command,
        IUseCase<InviteMemberCommand, InvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(invitation => TypedResults.Created(ApiRoutes.Members, invitation));
    }

    private static async Task<IResult> RevokeInvitationAsync(
        Guid invitationId,
        IUseCase<RevokeInvitationCommand, RevokedInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RevokeInvitationCommand(invitationId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> ChangeRoleAsync(
        Guid memberId,
        ChangeMemberRoleRequest request,
        IUseCase<ChangeMemberRoleCommand, MemberResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ChangeMemberRoleCommand(memberId, request.Role, request.InstructorId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RemoveAsync(
        Guid memberId,
        IUseCase<RemoveMemberCommand, RemovedMemberResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoveMemberCommand(memberId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
