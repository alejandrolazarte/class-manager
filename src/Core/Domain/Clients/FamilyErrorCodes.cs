namespace ClassManager.Core.Domain.Clients;

public static class FamilyErrorCodes
{
    public const string InvalidInvitation = "family_invitation.invalid";
    public const string AlreadyLinked = "family.already_linked";
    public const string EmailRequired = "family_invitation.email_required";

    public const string InvalidInvitationMessage = "The invitation link is invalid, expired or already used. Ask the school for a new one.";
}
