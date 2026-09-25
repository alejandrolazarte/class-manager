using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class BusinessTokenSubjectResolver(IBusinessMemberRepository businessMemberRepository) : ITokenSubjectResolver
{
    public async Task<TokenSubject?> ResolveAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        var member = await businessMemberRepository.FindByUserIdAsync(userId, cancellationToken);

        return member is null ? null : new TokenSubject(userId, email, member.TenantId, member.Role.ToString());
    }
}
