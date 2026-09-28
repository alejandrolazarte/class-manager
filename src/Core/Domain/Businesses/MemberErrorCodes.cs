namespace ClassManager.Core.Domain.Businesses;

public static class MemberErrorCodes
{
    public const string NoAccess = "member.no_access";
    public const string NotFound = "member.not_found";
    public const string InstructorRequired = "member.instructor_required";
    public const string InstructorTaken = "member.instructor_taken";
    public const string AlreadyMember = "member.already_member";
    public const string RoleNotAllowed = "member.role_not_allowed";
    public const string OwnMembership = "member.own_membership";
    public const string InvitationNotFound = "invitation.not_found";
    public const string InvalidInvitation = "invitation.invalid";
    public const string NotYours = "member.not_yours";

    public const string NoAccessMessage = "The current user has no access to this business.";
    public const string NotYoursMessage = "You can only do this for your own classes and students.";
}
