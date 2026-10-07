using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteMembersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomRoles_TenantId_Name",
                table: "CustomRoles");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_InstructorId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_UserId",
                table: "BusinessMembers");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "CustomRoles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "BusinessMembers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BusinessMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_TenantId_Name",
                table: "CustomRoles",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomRoles_IsDeleted_DeletedOn",
                table: "CustomRoles",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_InstructorId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "InstructorId" },
                unique: true,
                filter: "[InstructorId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_UserId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BusinessMembers_IsDeleted_DeletedOn",
                table: "BusinessMembers",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomRoles_TenantId_Name",
                table: "CustomRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomRoles_IsDeleted_DeletedOn",
                table: "CustomRoles");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_InstructorId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_UserId",
                table: "BusinessMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BusinessMembers_IsDeleted_DeletedOn",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "CustomRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomRoles");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BusinessMembers");

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_TenantId_Name",
                table: "CustomRoles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_InstructorId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "InstructorId" },
                unique: true,
                filter: "[InstructorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_UserId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "UserId" },
                unique: true);
        }
    }
}
