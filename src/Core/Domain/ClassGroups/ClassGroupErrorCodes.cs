namespace ClassManager.Core.Domain.ClassGroups;

public static class ClassGroupErrorCodes
{
    public const string NotFound = "class_group.not_found";
    public const string InstructorBusy = "class_group.instructor_busy";
    public const string Inactive = "class_group.inactive";
    public const string Full = "class_group.full";
    public const string HasEnrollments = "class_group.has_enrollments";
    public const string CapacityBelowEnrolled = "class_group.capacity_below_enrolled";
    public const string ConflictingClassGroupIdDetail = "classGroupId";
    public const string CapacityDetail = "capacity";
    public const string EnrollmentCountDetail = "enrollmentCount";
}
