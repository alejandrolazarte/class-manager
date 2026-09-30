using System.Collections.Frozen;

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
        Roles.Manage,
        Brand.CreateBranches,
        Brand.ManageBrandOwners,
        Instructors.View,
        Instructors.Manage,
        ClassGroups.ViewOwn,
        ClassGroups.ViewAll,
        ClassGroups.Manage,
        Enrollments.View,
        Enrollments.ManageOwn,
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
        Payments.ViewOwn,
        Payments.ViewAll,
        Payments.Record,
        ClassPacks.View,
        ClassPacks.Manage,
        ClassPacks.Sell,
        Products.View,
        Products.Manage,
        Orders.ViewOwn,
        Orders.ViewAll,
        Orders.Manage,
        ImportExport.Run,
        Announcements.Manage,
    ];

    public static IReadOnlyList<string> BrandOnly { get; } =
        [Members.ManageBranchOwners, Brand.CreateBranches, Brand.ManageBrandOwners];

    public static IReadOnlySet<string> Assignable { get; } =
        All.Except(BrandOnly).ToFrozenSet(StringComparer.Ordinal);

    public static IReadOnlyDictionary<string, string> EveryInstructorPermissionByOwnPermission { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ClassGroups.ViewOwn] = ClassGroups.ViewAll,
            [Sessions.ViewOwn] = Sessions.ViewAll,
            [Attendance.RecordOwn] = Attendance.RecordAll,
            [PrivateLessons.ViewOwn] = PrivateLessons.ViewAll,
            [PrivateLessons.ManageOwn] = PrivateLessons.ManageAll,
            [Students.ViewOwn] = Students.ViewAll,
            [Payments.ViewOwn] = Payments.ViewAll,
            [Enrollments.ManageOwn] = Enrollments.Manage,
            [Orders.ViewOwn] = Orders.ViewAll,
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

    public static class Roles
    {
        public const string Manage = "roles.manage";
    }

    public static class Brand
    {
        public const string CreateBranches = "branches.create";
        public const string ManageBrandOwners = "brandOwners.manage";
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
        public const string ManageOwn = "enrollments.manage.own";
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
        public const string ViewOwn = "payments.view.own";
        public const string ViewAll = "payments.view.all";
        public const string Record = "payments.record";
    }

    public static class ClassPacks
    {
        public const string View = "classPacks.view";
        public const string Manage = "classPacks.manage";
        public const string Sell = "classPacks.sell";
    }

    public static class Products
    {
        public const string View = "products.view";
        public const string Manage = "products.manage";
    }

    public static class Orders
    {
        public const string ViewOwn = "orders.view.own";
        public const string ViewAll = "orders.view.all";
        public const string Manage = "orders.manage";
    }

    public static class ImportExport
    {
        public const string Run = "importExport.run";
    }

    public static class Announcements
    {
        public const string Manage = "announcements.manage";
    }
}
