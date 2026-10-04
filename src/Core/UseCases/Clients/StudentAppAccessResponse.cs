using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public enum StudentAppAccessStatus
{
    NotInvited,
    Invited,
    Active,
}

public sealed record StudentAppAccessResponse(StudentAppAccessStatus Status, string? InvitedEmail)
{
    public static readonly StudentAppAccessResponse NotInvited = new(StudentAppAccessStatus.NotInvited, null);

    public static readonly StudentAppAccessResponse Active = new(StudentAppAccessStatus.Active, null);

    public static StudentAppAccessResponse From(bool hasAccount, ClientInvitation? pendingInvitation)
    {
        if (hasAccount)
        {
            return Active;
        }

        return pendingInvitation is null ? NotInvited : new(StudentAppAccessStatus.Invited, pendingInvitation.Email);
    }
}
