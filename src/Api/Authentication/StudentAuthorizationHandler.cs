using ClassManager.Core.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace ClassManager.Api.Authentication;

internal sealed class StudentAuthorizationHandler(IStudentAppAccess studentAppAccess) : AuthorizationHandler<StudentRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StudentRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true || !AccountKinds.IsStudent(SessionKindClaim.Of(context.User)))
        {
            return;
        }

        if (await studentAppAccess.GetAsync(CancellationToken.None) is not null)
        {
            context.Succeed(requirement);
        }
    }
}
