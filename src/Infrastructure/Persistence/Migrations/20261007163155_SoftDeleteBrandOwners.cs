using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteBrandOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMembers_OrganizationId_UserId",
                table: "OrganizationMembers");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "OrganizationMembers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrganizationMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_OrganizationId_UserId",
                table: "OrganizationMembers",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrganizationMembers_IsDeleted_DeletedOn",
                table: "OrganizationMembers",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMembers_OrganizationId_UserId",
                table: "OrganizationMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrganizationMembers_IsDeleted_DeletedOn",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrganizationMembers");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_OrganizationId_UserId",
                table: "OrganizationMembers",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);
        }
    }
}
