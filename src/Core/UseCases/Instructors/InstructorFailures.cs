using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.UseCases.Instructors;

internal static class InstructorFailures
{
    private const string NotFoundMessage = "The instructor does not exist.";
    private const string NameTakenMessage = "Another instructor already has this name.";
    private const string InactiveMessage = "The instructor is inactive.";
    private const string HasActiveClassGroupsMessage = "The instructor still teaches active class groups.";

    public static ResultError NotFound() => new(InstructorErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError NameTaken(Guid? existingInstructorId) =>
        new(InstructorErrorCodes.NameTaken, NameTakenMessage, ErrorKind.Conflict)
        {
            Details = new Dictionary<string, object?> { [InstructorErrorCodes.ExistingInstructorIdDetail] = existingInstructorId },
        };

    public static ResultError Inactive(string fieldName) =>
        new(InstructorErrorCodes.Inactive, InactiveMessage, ErrorKind.Validation) { FieldName = fieldName };

    public static ResultError HasActiveClassGroups(int classGroupCount) =>
        new(InstructorErrorCodes.HasActiveClassGroups, HasActiveClassGroupsMessage, ErrorKind.Conflict)
        {
            Details = new Dictionary<string, object?> { [InstructorErrorCodes.ClassGroupCountDetail] = classGroupCount },
        };
}
