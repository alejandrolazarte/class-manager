using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "TeamNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "TeamNotifications",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "TeamNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Students",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "StockMovements",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PushSubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "PushSubscriptions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PushSubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ProductVariants",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProductVariants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ProductVariants",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ProductVariants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Products",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ProductImages",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ProductImages",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "PrivateLessonStudents",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PrivateLessonStudents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "PrivateLessonStudents",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PrivateLessonStudents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PrivateLessons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "PrivateLessons",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PrivateLessons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Payments",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PackBookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "PackBookings",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PackBookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Orders",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "OrderLines",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "OrderLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "OrderLines",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "OrderLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MemberPushSubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MemberPushSubscriptions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MemberPushSubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MemberInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MemberInvitations",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MemberInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MeasurementRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MeasurementRecords",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MeasurementRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "MeasurementGoals",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MeasurementGoals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MeasurementGoals",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MeasurementGoals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MeasurementDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MeasurementDefinitions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MeasurementDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MakeupBookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MakeupBookings",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MakeupBookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Instructors",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Instructors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Instructors",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Instructors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Enrollments",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Documents",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "DefaultMonthlyFeeChanges",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "CustomRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "CustomRoles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "CustomRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Clients",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClientInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClientInvitations",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClientInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClientBillingPlanChanges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClientBillingPlanChanges",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClientBillingPlanChanges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClientAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClientAccounts",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClientAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassPacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassPacks",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassPacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassPackPurchases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassPackPurchases",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassPackPurchases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ClassPackImages",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassPackImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassPackImages",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassPackImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ClassPackClassGroups",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassPackClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassPackClassGroups",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassPackClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ClassGroups",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassGroups",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ClassGroupMaterialFiles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassGroupMaterialFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClassGroupMaterialFiles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassGroupMaterialFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ClassFeedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ClassFeedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "BusinessMembers",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BusinessMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "BusinessMembers",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BusinessMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "BrandLogos",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BrandLogos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BrandLogos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Attendances",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Attendances",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AnnouncementViews",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AnnouncementViews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AnnouncementViews",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AnnouncementViews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Announcements",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Announcements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Announcements",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Announcements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AnnouncementReplies",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AnnouncementReplies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AnnouncementReplies",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AnnouncementReplies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AnnouncementImages",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AnnouncementImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AnnouncementImages",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AnnouncementImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AnnouncementClassGroups",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AnnouncementClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AnnouncementClassGroups",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AnnouncementClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AchievementLevels",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AchievementLevels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AchievementLevels",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AchievementLevels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AbsenceNotices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AbsenceNotices",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AbsenceNotices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByUserId",
                table: "Users",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedByUserId",
                table: "Users",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamNotifications_CreatedByUserId",
                table: "TeamNotifications",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamNotifications_UpdatedByUserId",
                table: "TeamNotifications",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CreatedByUserId",
                table: "Students",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UpdatedByUserId",
                table: "Students",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CreatedByUserId",
                table: "StockMovements",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_UpdatedByUserId",
                table: "StockMovements",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_CreatedByUserId",
                table: "PushSubscriptions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UpdatedByUserId",
                table: "PushSubscriptions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_CreatedByUserId",
                table: "ProductVariants",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_UpdatedByUserId",
                table: "ProductVariants",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedByUserId",
                table: "Products",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedByUserId",
                table: "Products",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_CreatedByUserId",
                table: "ProductImages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_UpdatedByUserId",
                table: "ProductImages",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateLessonStudents_CreatedByUserId",
                table: "PrivateLessonStudents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateLessonStudents_UpdatedByUserId",
                table: "PrivateLessonStudents",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateLessons_CreatedByUserId",
                table: "PrivateLessons",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateLessons_UpdatedByUserId",
                table: "PrivateLessons",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedByUserId",
                table: "Payments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UpdatedByUserId",
                table: "Payments",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PackBookings_CreatedByUserId",
                table: "PackBookings",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PackBookings_UpdatedByUserId",
                table: "PackBookings",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UpdatedByUserId",
                table: "Orders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_CreatedByUserId",
                table: "OrderLines",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_UpdatedByUserId",
                table: "OrderLines",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPushSubscriptions_CreatedByUserId",
                table: "MemberPushSubscriptions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPushSubscriptions_UpdatedByUserId",
                table: "MemberPushSubscriptions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitations_CreatedByUserId",
                table: "MemberInvitations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitations_UpdatedByUserId",
                table: "MemberInvitations",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_CreatedByUserId",
                table: "MeasurementRecords",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_UpdatedByUserId",
                table: "MeasurementRecords",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementGoals_CreatedByUserId",
                table: "MeasurementGoals",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementGoals_UpdatedByUserId",
                table: "MeasurementGoals",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitions_CreatedByUserId",
                table: "MeasurementDefinitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitions_UpdatedByUserId",
                table: "MeasurementDefinitions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MakeupBookings_CreatedByUserId",
                table: "MakeupBookings",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MakeupBookings_UpdatedByUserId",
                table: "MakeupBookings",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_CreatedByUserId",
                table: "Instructors",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_UpdatedByUserId",
                table: "Instructors",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CreatedByUserId",
                table: "Enrollments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UpdatedByUserId",
                table: "Enrollments",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedByUserId",
                table: "Documents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UpdatedByUserId",
                table: "Documents",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_CreatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_CreatedByUserId",
                table: "CustomRoles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_UpdatedByUserId",
                table: "CustomRoles",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CreatedByUserId",
                table: "Clients",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UpdatedByUserId",
                table: "Clients",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvitations_CreatedByUserId",
                table: "ClientInvitations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvitations_UpdatedByUserId",
                table: "ClientInvitations",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_CreatedByUserId",
                table: "ClientBillingPlanChanges",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_UpdatedByUserId",
                table: "ClientBillingPlanChanges",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_CreatedByUserId",
                table: "ClientAccounts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_UpdatedByUserId",
                table: "ClientAccounts",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_CreatedByUserId",
                table: "ClassSessions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_UpdatedByUserId",
                table: "ClassSessions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPacks_CreatedByUserId",
                table: "ClassPacks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPacks_UpdatedByUserId",
                table: "ClassPacks",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_CreatedByUserId",
                table: "ClassPackPurchases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_UpdatedByUserId",
                table: "ClassPackPurchases",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackImages_CreatedByUserId",
                table: "ClassPackImages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackImages_UpdatedByUserId",
                table: "ClassPackImages",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackClassGroups_CreatedByUserId",
                table: "ClassPackClassGroups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackClassGroups_UpdatedByUserId",
                table: "ClassPackClassGroups",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroups_CreatedByUserId",
                table: "ClassGroups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroups_UpdatedByUserId",
                table: "ClassGroups",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroupMaterialFiles_CreatedByUserId",
                table: "ClassGroupMaterialFiles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroupMaterialFiles_UpdatedByUserId",
                table: "ClassGroupMaterialFiles",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_CreatedByUserId",
                table: "ClassFeedbacks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_UpdatedByUserId",
                table: "ClassFeedbacks",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_CreatedByUserId",
                table: "BusinessMembers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_UpdatedByUserId",
                table: "BusinessMembers",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandLogos_CreatedByUserId",
                table: "BrandLogos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandLogos_UpdatedByUserId",
                table: "BrandLogos",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CreatedByUserId",
                table: "Attendances",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UpdatedByUserId",
                table: "Attendances",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementViews_CreatedByUserId",
                table: "AnnouncementViews",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementViews_UpdatedByUserId",
                table: "AnnouncementViews",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CreatedByUserId",
                table: "Announcements",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_UpdatedByUserId",
                table: "Announcements",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementReplies_CreatedByUserId",
                table: "AnnouncementReplies",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementReplies_UpdatedByUserId",
                table: "AnnouncementReplies",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementImages_CreatedByUserId",
                table: "AnnouncementImages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementImages_UpdatedByUserId",
                table: "AnnouncementImages",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementClassGroups_CreatedByUserId",
                table: "AnnouncementClassGroups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementClassGroups_UpdatedByUserId",
                table: "AnnouncementClassGroups",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementLevels_CreatedByUserId",
                table: "AchievementLevels",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementLevels_UpdatedByUserId",
                table: "AchievementLevels",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceNotices_CreatedByUserId",
                table: "AbsenceNotices",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceNotices_UpdatedByUserId",
                table: "AbsenceNotices",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceNotices_Users_CreatedByUserId",
                table: "AbsenceNotices",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceNotices_Users_UpdatedByUserId",
                table: "AbsenceNotices",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementLevels_Users_CreatedByUserId",
                table: "AchievementLevels",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementLevels_Users_UpdatedByUserId",
                table: "AchievementLevels",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementClassGroups_Users_CreatedByUserId",
                table: "AnnouncementClassGroups",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementClassGroups_Users_UpdatedByUserId",
                table: "AnnouncementClassGroups",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementImages_Users_CreatedByUserId",
                table: "AnnouncementImages",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementImages_Users_UpdatedByUserId",
                table: "AnnouncementImages",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementReplies_Users_CreatedByUserId",
                table: "AnnouncementReplies",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementReplies_Users_UpdatedByUserId",
                table: "AnnouncementReplies",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_CreatedByUserId",
                table: "Announcements",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_UpdatedByUserId",
                table: "Announcements",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementViews_Users_CreatedByUserId",
                table: "AnnouncementViews",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementViews_Users_UpdatedByUserId",
                table: "AnnouncementViews",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_CreatedByUserId",
                table: "Attendances",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_UpdatedByUserId",
                table: "Attendances",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandLogos_Users_CreatedByUserId",
                table: "BrandLogos",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandLogos_Users_UpdatedByUserId",
                table: "BrandLogos",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMembers_Users_CreatedByUserId",
                table: "BusinessMembers",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMembers_Users_UpdatedByUserId",
                table: "BusinessMembers",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassFeedbacks_Users_CreatedByUserId",
                table: "ClassFeedbacks",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassFeedbacks_Users_UpdatedByUserId",
                table: "ClassFeedbacks",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroupMaterialFiles_Users_CreatedByUserId",
                table: "ClassGroupMaterialFiles",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroupMaterialFiles_Users_UpdatedByUserId",
                table: "ClassGroupMaterialFiles",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroups_Users_CreatedByUserId",
                table: "ClassGroups",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroups_Users_UpdatedByUserId",
                table: "ClassGroups",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackClassGroups_Users_CreatedByUserId",
                table: "ClassPackClassGroups",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackClassGroups_Users_UpdatedByUserId",
                table: "ClassPackClassGroups",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackImages_Users_CreatedByUserId",
                table: "ClassPackImages",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackImages_Users_UpdatedByUserId",
                table: "ClassPackImages",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackPurchases_Users_CreatedByUserId",
                table: "ClassPackPurchases",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackPurchases_Users_UpdatedByUserId",
                table: "ClassPackPurchases",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPacks_Users_CreatedByUserId",
                table: "ClassPacks",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPacks_Users_UpdatedByUserId",
                table: "ClassPacks",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Users_CreatedByUserId",
                table: "ClassSessions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Users_UpdatedByUserId",
                table: "ClassSessions",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAccounts_Users_CreatedByUserId",
                table: "ClientAccounts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAccounts_Users_UpdatedByUserId",
                table: "ClientAccounts",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientBillingPlanChanges_Users_CreatedByUserId",
                table: "ClientBillingPlanChanges",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientBillingPlanChanges_Users_UpdatedByUserId",
                table: "ClientBillingPlanChanges",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvitations_Users_CreatedByUserId",
                table: "ClientInvitations",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvitations_Users_UpdatedByUserId",
                table: "ClientInvitations",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_CreatedByUserId",
                table: "Clients",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_UpdatedByUserId",
                table: "Clients",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomRoles_Users_CreatedByUserId",
                table: "CustomRoles",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomRoles_Users_UpdatedByUserId",
                table: "CustomRoles",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultMonthlyFeeChanges_Users_CreatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultMonthlyFeeChanges_Users_UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_CreatedByUserId",
                table: "Documents",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UpdatedByUserId",
                table: "Documents",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_CreatedByUserId",
                table: "Enrollments",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_UpdatedByUserId",
                table: "Enrollments",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Users_CreatedByUserId",
                table: "Instructors",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Users_UpdatedByUserId",
                table: "Instructors",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MakeupBookings_Users_CreatedByUserId",
                table: "MakeupBookings",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MakeupBookings_Users_UpdatedByUserId",
                table: "MakeupBookings",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDefinitions_Users_CreatedByUserId",
                table: "MeasurementDefinitions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDefinitions_Users_UpdatedByUserId",
                table: "MeasurementDefinitions",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementGoals_Users_CreatedByUserId",
                table: "MeasurementGoals",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementGoals_Users_UpdatedByUserId",
                table: "MeasurementGoals",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementRecords_Users_CreatedByUserId",
                table: "MeasurementRecords",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementRecords_Users_UpdatedByUserId",
                table: "MeasurementRecords",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInvitations_Users_CreatedByUserId",
                table: "MemberInvitations",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInvitations_Users_UpdatedByUserId",
                table: "MemberInvitations",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberPushSubscriptions_Users_CreatedByUserId",
                table: "MemberPushSubscriptions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberPushSubscriptions_Users_UpdatedByUserId",
                table: "MemberPushSubscriptions",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Users_CreatedByUserId",
                table: "OrderLines",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Users_UpdatedByUserId",
                table: "OrderLines",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UpdatedByUserId",
                table: "Orders",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackBookings_Users_CreatedByUserId",
                table: "PackBookings",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackBookings_Users_UpdatedByUserId",
                table: "PackBookings",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_CreatedByUserId",
                table: "Payments",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UpdatedByUserId",
                table: "Payments",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateLessons_Users_CreatedByUserId",
                table: "PrivateLessons",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateLessons_Users_UpdatedByUserId",
                table: "PrivateLessons",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateLessonStudents_Users_CreatedByUserId",
                table: "PrivateLessonStudents",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateLessonStudents_Users_UpdatedByUserId",
                table: "PrivateLessonStudents",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Users_CreatedByUserId",
                table: "ProductImages",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Users_UpdatedByUserId",
                table: "ProductImages",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatedByUserId",
                table: "Products",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UpdatedByUserId",
                table: "Products",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Users_CreatedByUserId",
                table: "ProductVariants",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Users_UpdatedByUserId",
                table: "ProductVariants",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PushSubscriptions_Users_CreatedByUserId",
                table: "PushSubscriptions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PushSubscriptions_Users_UpdatedByUserId",
                table: "PushSubscriptions",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Users_CreatedByUserId",
                table: "StockMovements",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Users_UpdatedByUserId",
                table: "StockMovements",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_CreatedByUserId",
                table: "Students",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UpdatedByUserId",
                table: "Students",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamNotifications_Users_CreatedByUserId",
                table: "TeamNotifications",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamNotifications_Users_UpdatedByUserId",
                table: "TeamNotifications",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedByUserId",
                table: "Users",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UpdatedByUserId",
                table: "Users",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("UPDATE [AbsenceNotices] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [AchievementLevels] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [AnnouncementClassGroups] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [AnnouncementImages] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [AnnouncementReplies] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [AnnouncementViews] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Announcements] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Attendances] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [BusinessMembers] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassGroupMaterialFiles] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassGroups] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassPackClassGroups] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassPackImages] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassPackPurchases] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassPacks] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClassSessions] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClientAccounts] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClientBillingPlanChanges] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ClientInvitations] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Clients] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [CustomRoles] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [DefaultMonthlyFeeChanges] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Documents] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Enrollments] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Instructors] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [MakeupBookings] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [MeasurementDefinitions] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [MeasurementGoals] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [MeasurementRecords] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [MemberInvitations] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [MemberPushSubscriptions] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [OrderLines] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Orders] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [PackBookings] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Payments] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [PrivateLessonStudents] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [PrivateLessons] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ProductImages] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [ProductVariants] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Products] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [PushSubscriptions] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [StockMovements] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Students] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [TeamNotifications] SET [UpdatedAt] = [CreatedAt]");
            migrationBuilder.Sql("UPDATE [Users] SET [UpdatedAt] = [CreatedAt]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceNotices_Users_CreatedByUserId",
                table: "AbsenceNotices");

            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceNotices_Users_UpdatedByUserId",
                table: "AbsenceNotices");

            migrationBuilder.DropForeignKey(
                name: "FK_AchievementLevels_Users_CreatedByUserId",
                table: "AchievementLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_AchievementLevels_Users_UpdatedByUserId",
                table: "AchievementLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementClassGroups_Users_CreatedByUserId",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementClassGroups_Users_UpdatedByUserId",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementImages_Users_CreatedByUserId",
                table: "AnnouncementImages");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementImages_Users_UpdatedByUserId",
                table: "AnnouncementImages");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementReplies_Users_CreatedByUserId",
                table: "AnnouncementReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementReplies_Users_UpdatedByUserId",
                table: "AnnouncementReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_CreatedByUserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_UpdatedByUserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementViews_Users_CreatedByUserId",
                table: "AnnouncementViews");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementViews_Users_UpdatedByUserId",
                table: "AnnouncementViews");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_CreatedByUserId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_UpdatedByUserId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandLogos_Users_CreatedByUserId",
                table: "BrandLogos");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandLogos_Users_UpdatedByUserId",
                table: "BrandLogos");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMembers_Users_CreatedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMembers_Users_UpdatedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassFeedbacks_Users_CreatedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassFeedbacks_Users_UpdatedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroupMaterialFiles_Users_CreatedByUserId",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroupMaterialFiles_Users_UpdatedByUserId",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroups_Users_CreatedByUserId",
                table: "ClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroups_Users_UpdatedByUserId",
                table: "ClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackClassGroups_Users_CreatedByUserId",
                table: "ClassPackClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackClassGroups_Users_UpdatedByUserId",
                table: "ClassPackClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackImages_Users_CreatedByUserId",
                table: "ClassPackImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackImages_Users_UpdatedByUserId",
                table: "ClassPackImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackPurchases_Users_CreatedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackPurchases_Users_UpdatedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPacks_Users_CreatedByUserId",
                table: "ClassPacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPacks_Users_UpdatedByUserId",
                table: "ClassPacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Users_CreatedByUserId",
                table: "ClassSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Users_UpdatedByUserId",
                table: "ClassSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientAccounts_Users_CreatedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientAccounts_Users_UpdatedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientBillingPlanChanges_Users_CreatedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientBillingPlanChanges_Users_UpdatedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvitations_Users_CreatedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvitations_Users_UpdatedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_CreatedByUserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_UpdatedByUserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomRoles_Users_CreatedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomRoles_Users_UpdatedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultMonthlyFeeChanges_Users_CreatedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultMonthlyFeeChanges_Users_UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_CreatedByUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_UpdatedByUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_CreatedByUserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_UpdatedByUserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Users_CreatedByUserId",
                table: "Instructors");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Users_UpdatedByUserId",
                table: "Instructors");

            migrationBuilder.DropForeignKey(
                name: "FK_MakeupBookings_Users_CreatedByUserId",
                table: "MakeupBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_MakeupBookings_Users_UpdatedByUserId",
                table: "MakeupBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDefinitions_Users_CreatedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDefinitions_Users_UpdatedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementGoals_Users_CreatedByUserId",
                table: "MeasurementGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementGoals_Users_UpdatedByUserId",
                table: "MeasurementGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementRecords_Users_CreatedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementRecords_Users_UpdatedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInvitations_Users_CreatedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInvitations_Users_UpdatedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberPushSubscriptions_Users_CreatedByUserId",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberPushSubscriptions_Users_UpdatedByUserId",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Users_CreatedByUserId",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Users_UpdatedByUserId",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UpdatedByUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PackBookings_Users_CreatedByUserId",
                table: "PackBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_PackBookings_Users_UpdatedByUserId",
                table: "PackBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateLessons_Users_CreatedByUserId",
                table: "PrivateLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateLessons_Users_UpdatedByUserId",
                table: "PrivateLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateLessonStudents_Users_CreatedByUserId",
                table: "PrivateLessonStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateLessonStudents_Users_UpdatedByUserId",
                table: "PrivateLessonStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Users_CreatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Users_UpdatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatedByUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Users_CreatedByUserId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Users_UpdatedByUserId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_PushSubscriptions_Users_CreatedByUserId",
                table: "PushSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PushSubscriptions_Users_UpdatedByUserId",
                table: "PushSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Users_CreatedByUserId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Users_UpdatedByUserId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_CreatedByUserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UpdatedByUserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamNotifications_Users_CreatedByUserId",
                table: "TeamNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamNotifications_Users_UpdatedByUserId",
                table: "TeamNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedByUserId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UpdatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UpdatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TeamNotifications_CreatedByUserId",
                table: "TeamNotifications");

            migrationBuilder.DropIndex(
                name: "IX_TeamNotifications_UpdatedByUserId",
                table: "TeamNotifications");

            migrationBuilder.DropIndex(
                name: "IX_Students_CreatedByUserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_UpdatedByUserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_CreatedByUserId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_UpdatedByUserId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_CreatedByUserId",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UpdatedByUserId",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_CreatedByUserId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_UpdatedByUserId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreatedByUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_CreatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_UpdatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_PrivateLessonStudents_CreatedByUserId",
                table: "PrivateLessonStudents");

            migrationBuilder.DropIndex(
                name: "IX_PrivateLessonStudents_UpdatedByUserId",
                table: "PrivateLessonStudents");

            migrationBuilder.DropIndex(
                name: "IX_PrivateLessons_CreatedByUserId",
                table: "PrivateLessons");

            migrationBuilder.DropIndex(
                name: "IX_PrivateLessons_UpdatedByUserId",
                table: "PrivateLessons");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_PackBookings_CreatedByUserId",
                table: "PackBookings");

            migrationBuilder.DropIndex(
                name: "IX_PackBookings_UpdatedByUserId",
                table: "PackBookings");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UpdatedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_CreatedByUserId",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_UpdatedByUserId",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_MemberPushSubscriptions_CreatedByUserId",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_MemberPushSubscriptions_UpdatedByUserId",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_MemberInvitations_CreatedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropIndex(
                name: "IX_MemberInvitations_UpdatedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementRecords_CreatedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementRecords_UpdatedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementGoals_CreatedByUserId",
                table: "MeasurementGoals");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementGoals_UpdatedByUserId",
                table: "MeasurementGoals");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementDefinitions_CreatedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementDefinitions_UpdatedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_MakeupBookings_CreatedByUserId",
                table: "MakeupBookings");

            migrationBuilder.DropIndex(
                name: "IX_MakeupBookings_UpdatedByUserId",
                table: "MakeupBookings");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_CreatedByUserId",
                table: "Instructors");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_UpdatedByUserId",
                table: "Instructors");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CreatedByUserId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_UpdatedByUserId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CreatedByUserId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_UpdatedByUserId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_DefaultMonthlyFeeChanges_CreatedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropIndex(
                name: "IX_DefaultMonthlyFeeChanges_UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropIndex(
                name: "IX_CustomRoles_CreatedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropIndex(
                name: "IX_CustomRoles_UpdatedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CreatedByUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UpdatedByUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvitations_CreatedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvitations_UpdatedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ClientBillingPlanChanges_CreatedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClientBillingPlanChanges_UpdatedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_CreatedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_UpdatedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_CreatedByUserId",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_UpdatedByUserId",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassPacks_CreatedByUserId",
                table: "ClassPacks");

            migrationBuilder.DropIndex(
                name: "IX_ClassPacks_UpdatedByUserId",
                table: "ClassPacks");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_CreatedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_UpdatedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackImages_CreatedByUserId",
                table: "ClassPackImages");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackImages_UpdatedByUserId",
                table: "ClassPackImages");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackClassGroups_CreatedByUserId",
                table: "ClassPackClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackClassGroups_UpdatedByUserId",
                table: "ClassPackClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroups_CreatedByUserId",
                table: "ClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroups_UpdatedByUserId",
                table: "ClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroupMaterialFiles_CreatedByUserId",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroupMaterialFiles_UpdatedByUserId",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_CreatedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_UpdatedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_CreatedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_UpdatedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_BrandLogos_CreatedByUserId",
                table: "BrandLogos");

            migrationBuilder.DropIndex(
                name: "IX_BrandLogos_UpdatedByUserId",
                table: "BrandLogos");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_CreatedByUserId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_UpdatedByUserId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementViews_CreatedByUserId",
                table: "AnnouncementViews");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementViews_UpdatedByUserId",
                table: "AnnouncementViews");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CreatedByUserId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_UpdatedByUserId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementReplies_CreatedByUserId",
                table: "AnnouncementReplies");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementReplies_UpdatedByUserId",
                table: "AnnouncementReplies");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementImages_CreatedByUserId",
                table: "AnnouncementImages");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementImages_UpdatedByUserId",
                table: "AnnouncementImages");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementClassGroups_CreatedByUserId",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementClassGroups_UpdatedByUserId",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_AchievementLevels_CreatedByUserId",
                table: "AchievementLevels");

            migrationBuilder.DropIndex(
                name: "IX_AchievementLevels_UpdatedByUserId",
                table: "AchievementLevels");

            migrationBuilder.DropIndex(
                name: "IX_AbsenceNotices_CreatedByUserId",
                table: "AbsenceNotices");

            migrationBuilder.DropIndex(
                name: "IX_AbsenceNotices_UpdatedByUserId",
                table: "AbsenceNotices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TeamNotifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TeamNotifications");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "TeamNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PrivateLessonStudents");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PrivateLessonStudents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PrivateLessonStudents");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PrivateLessonStudents");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PrivateLessons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PrivateLessons");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PrivateLessons");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PackBookings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PackBookings");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PackBookings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MemberPushSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MemberInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MeasurementRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MeasurementGoals");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MeasurementGoals");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MeasurementGoals");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MeasurementGoals");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MeasurementDefinitions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MakeupBookings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MakeupBookings");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MakeupBookings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CustomRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClassPackImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassPackImages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassPackImages");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassPackImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClassPackClassGroups");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassPackClassGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassPackClassGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassPackClassGroups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassGroupMaterialFiles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BrandLogos");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BrandLogos");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BrandLogos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnnouncementViews");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AnnouncementViews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AnnouncementViews");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AnnouncementViews");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnnouncementReplies");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AnnouncementReplies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AnnouncementReplies");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AnnouncementReplies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnnouncementImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AnnouncementImages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AnnouncementImages");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AnnouncementImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AnnouncementClassGroups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AchievementLevels");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AchievementLevels");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AchievementLevels");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AchievementLevels");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AbsenceNotices");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AbsenceNotices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AbsenceNotices");
        }
    }
}
