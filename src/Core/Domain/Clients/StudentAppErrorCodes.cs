namespace ClassManager.Core.Domain.Clients;

public static class StudentAppErrorCodes
{
    public const string InvalidInvitation = "student_invitation.invalid";
    public const string AlreadyLinked = "student.already_linked";
    public const string EmailRequired = "student_invitation.email_required";
    public const string AlreadyUsesTheApp = "student_invitation.already_uses_the_app";
    public const string BirthDateRequired = "student_invitation.birth_date_required";

    public const string InvalidInvitationMessage = "The invitation link is invalid, expired or already used. Ask the school for a new one.";
}
