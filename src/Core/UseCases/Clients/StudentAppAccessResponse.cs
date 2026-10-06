using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public enum StudentAppAccessStatus
{
    NotInvited,
    Invited,
    Active,
}

public sealed record StudentAppAccessResponse(StudentAppAccessStatus Status, string? InvitedEmail, string? SignInEmail = null)
{
    public static readonly StudentAppAccessResponse NotInvited = new(StudentAppAccessStatus.NotInvited, null);

    public static StudentAppAccessResponse From(bool hasAccount, string? signInEmail, ClientInvitation? pendingInvitation)
    {
        if (hasAccount)
        {
            return new(StudentAppAccessStatus.Active, null, signInEmail);
        }

        return pendingInvitation is null ? NotInvited : new(StudentAppAccessStatus.Invited, pendingInvitation.Email);
    }
}
