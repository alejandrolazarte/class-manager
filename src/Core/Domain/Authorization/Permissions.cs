namespace ClassManager.Core.Domain.Authorization;

public static class Permissions
{
    public static IReadOnlyList<string> All { get; } =
    [
        Business.View,
        Business.Manage,
        Instructors.View,
        Instructors.Manage,
        ClassGroups.ViewAll,
        ClassGroups.Manage,
        Enrollments.View,
        Enrollments.Manage,
        Sessions.ViewAll,
        Sessions.Manage,
        Attendance.RecordAll,
        PrivateLessons.ViewAll,
        PrivateLessons.ManageAll,
        Students.ViewAll,
        Students.Manage,
        Payments.ViewAll,
        Payments.Record,
        ClassPacks.View,
        ClassPacks.Manage,
        ClassPacks.Sell,
        ImportExport.Run,
    ];

    public static class Business
    {
        public const string View = "business.view";
        public const string Manage = "business.manage";
    }

    public static class Instructors
    {
        public const string View = "instructors.view";
        public const string Manage = "instructors.manage";
    }

    public static class ClassGroups
    {
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
        public const string ViewAll = "sessions.view.all";
        public const string Manage = "sessions.manage";
    }

    public static class Attendance
    {
        public const string RecordAll = "attendance.record.all";
    }

    public static class PrivateLessons
    {
        public const string ViewAll = "privateLessons.view.all";
        public const string ManageAll = "privateLessons.manage.all";
    }

    public static class Students
    {
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
