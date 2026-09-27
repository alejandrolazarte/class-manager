using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPackLessonsAndTrials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrial",
                table: "PrivateLessons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TrialPrice",
                table: "PrivateLessons",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassDurationMinutes",
                table: "ClassPacks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialUrl",
                table: "ClassPacks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassDurationMinutes",
                table: "ClassPackPurchases",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialUrl",
                table: "ClassPackPurchases",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TrialLessonId",
                table: "ClassPackPurchases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "TrialLessonId" },
                unique: true,
                filter: "[TrialLessonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TrialLessonId",
                table: "ClassPackPurchases",
                column: "TrialLessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackPurchases_PrivateLessons_TrialLessonId",
                table: "ClassPackPurchases",
                column: "TrialLessonId",
                principalTable: "PrivateLessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackPurchases_PrivateLessons_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "IsTrial",
                table: "PrivateLessons");

            migrationBuilder.DropColumn(
                name: "TrialPrice",
                table: "PrivateLessons");

            migrationBuilder.DropColumn(
                name: "ClassDurationMinutes",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "MaterialUrl",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "ClassDurationMinutes",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "MaterialUrl",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "TrialLessonId",
                table: "ClassPackPurchases");
        }
    }
}
