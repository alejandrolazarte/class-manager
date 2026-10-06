using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeletePaymentsPurchasesAnnouncementsFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "Payments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "ClassPackPurchases",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "ClassFeedbacks",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "Announcements",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "TrialLessonId" },
                unique: true,
                filter: "[TrialLessonId] IS NOT NULL AND [DeletedOn] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true,
                filter: "[DeletedOn] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Announcements");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "TrialLessonId" },
                unique: true,
                filter: "[TrialLessonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true);
        }
    }
}
