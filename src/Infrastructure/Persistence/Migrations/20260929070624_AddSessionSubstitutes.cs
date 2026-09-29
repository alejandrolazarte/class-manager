using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionSubstitutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubstituteInstructorId",
                table: "ClassSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_SubstituteInstructorId",
                table: "ClassSessions",
                column: "SubstituteInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_TenantId_SubstituteInstructorId_Date",
                table: "ClassSessions",
                columns: new[] { "TenantId", "SubstituteInstructorId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Instructors_SubstituteInstructorId",
                table: "ClassSessions",
                column: "SubstituteInstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Instructors_SubstituteInstructorId",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_SubstituteInstructorId",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_TenantId_SubstituteInstructorId_Date",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "SubstituteInstructorId",
                table: "ClassSessions");
        }
    }
}
