using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Roles;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Roles;

internal sealed class RoleUseCaseBuilder
{
    public Mock<ICustomRoleRepository> CustomRoles { get; } = new();
    public Mock<IBusinessMemberRepository> Members { get; } = new();
    public Mock<IMemberInvitationRepository> Invitations { get; } = new();
    public Mock<ICurrentMember> CurrentMember { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public List<CustomRole> AddedRoles { get; } = [];
    public CustomRole ExistingRole { get; } =
        CustomRole.Create("Recepción", [Permissions.Business.View, Permissions.Students.ViewAll], null, TestData.Now).Value!;

    public RoleUseCaseBuilder()
    {
        CustomRoles.Setup(repository => repository.Add(It.IsAny<CustomRole>())).Callback<CustomRole>(AddedRoles.Add);
        CustomRoles.Setup(repository => repository.GetForUpdateAsync(ExistingRole.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingRole);
        CustomRoles.Setup(repository => repository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync([ExistingRole]);
        Members.Setup(repository => repository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Members.Setup(repository => repository.ListByCustomRoleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Invitations
            .Setup(repository => repository.ListPendingByCustomRoleAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        ActAs(SystemRolePermissions.Of(BusinessRole.BranchOwner), BusinessRole.BranchOwner, customRoleId: null);
    }

    public void ActAs(IReadOnlySet<string> permissions, BusinessRole role, Guid? customRoleId) =>
        CurrentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(Guid.CreateVersion7(), Guid.CreateVersion7(), role, null, IsBrandOwner: false, permissions, customRoleId));

    public static BusinessMember MemberWith(CustomRole role, Guid? instructorId) =>
        BusinessMember.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), MemberRole.Custom(role), instructorId).Value!;

    public CreateRoleUseCase BuildCreate() => new(CustomRoles.Object, CurrentMember.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

    public UpdateRoleUseCase BuildUpdate() =>
        new(CustomRoles.Object, Members.Object, Invitations.Object, CurrentMember.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

    public DeleteRoleUseCase BuildDelete() =>
        new(CustomRoles.Object, Members.Object, Invitations.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));
}
