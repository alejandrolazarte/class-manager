namespace ClassManager.Core.Domain.Authorization;

public static class Permissions
{
    public static IReadOnlyList<string> All { get; } =
    [
        Business.View,
        Business.Manage,
        Members.View,
        Members.Manage,
        Members.ManageBranchOwners,
        Instructors.View,
        Instructors.Manage,
        ClassGroups.ViewOwn,
        ClassGroups.ViewAll,
        ClassGroups.Manage,
        Enrollments.View,
        Enrollments.Manage,
        Sessions.ViewOwn,
        Sessions.ViewAll,
        Sessions.Manage,
        Attendance.RecordOwn,
        Attendance.RecordAll,
        PrivateLessons.ViewOwn,
        PrivateLessons.ViewAll,
        PrivateLessons.ManageOwn,
        PrivateLessons.ManageAll,
        Students.ViewOwn,
        Students.ViewAll,
        Students.Manage,
        Payments.ViewAll,
        Payments.Record,
        ClassPacks.View,
        ClassPacks.Manage,
        ClassPacks.Sell,
        ImportExport.Run,
    ];

    public static IReadOnlyList<string> BrandOnly { get; } = [Members.ManageBranchOwners];

    public static IReadOnlyDictionary<string, string> EveryInstructorPermissionByOwnPermission { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ClassGroups.ViewOwn] = ClassGroups.ViewAll,
            [Sessions.ViewOwn] = Sessions.ViewAll,
            [Attendance.RecordOwn] = Attendance.RecordAll,
            [PrivateLessons.ViewOwn] = PrivateLessons.ViewAll,
            [PrivateLessons.ManageOwn] = PrivateLessons.ManageAll,
            [Students.ViewOwn] = Students.ViewAll,
        };

    public static class Business
    {
        public const string View = "business.view";
        public const string Manage = "business.manage";
    }

    public static class Members
    {
        public const string View = "members.view";
        public const string Manage = "members.manage";
        public const string ManageBranchOwners = "branchOwners.manage";
    }

    public static class Instructors
    {
        public const string View = "instructors.view";
        public const string Manage = "instructors.manage";
    }

    public static class ClassGroups
    {
        public const string ViewOwn = "classGroups.view.own";
        public const string ViewAll = "classGroups.view.all";
        public const string Manage = "classGroups.manage";
    }

    public static class Enrollments
    {
        public const string View = "enrollments.view";
        public const string Manage = "enrollments.manage";
    }

    public static class Sessions
    {
        public const string ViewOwn = "sessions.view.own";
        public const string ViewAll = "sessions.view.all";
        public const string Manage = "sessions.manage";
    }

    public static class Attendance
    {
        public const string RecordOwn = "attendance.record.own";
        public const string RecordAll = "attendance.record.all";
    }

    public static class PrivateLessons
    {
        public const string ViewOwn = "privateLessons.view.own";
        public const string ViewAll = "privateLessons.view.all";
        public const string ManageOwn = "privateLessons.manage.own";
        public const string ManageAll = "privateLessons.manage.all";
    }

    public static class Students
    {
        public const string ViewOwn = "students.view.own";
        public const string ViewAll = "students.view.all";
        public const string Manage = "students.manage";
    }

    public static class Payments
    {
        public const string ViewAll = "payments.view.all";
        public const string Record = "payments.record";
    }

    public static class ClassPacks
    {
        public const string View = "classPacks.view";
        public const string Manage = "classPacks.manage";
        public const string Sell = "classPacks.sell";
    }

    public static class ImportExport
    {
        public const string Run = "importExport.run";
    }
}
