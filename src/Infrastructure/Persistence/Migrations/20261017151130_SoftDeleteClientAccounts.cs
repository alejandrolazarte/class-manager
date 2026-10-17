using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteClientAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_TenantId_UserId",
                table: "ClientAccounts");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ClientAccounts",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ClientAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClientAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_DeletedByUserId",
                table: "ClientAccounts",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_TenantId_UserId",
                table: "ClientAccounts",
                columns: new[] { "TenantId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClientAccounts_IsDeleted_DeletedAt",
                table: "ClientAccounts",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAccounts_Users_DeletedByUserId",
                table: "ClientAccounts",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientAccounts_Users_DeletedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_DeletedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_TenantId_UserId",
                table: "ClientAccounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClientAccounts_IsDeleted_DeletedAt",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClientAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_TenantId_UserId",
                table: "ClientAccounts",
                columns: new[] { "TenantId", "UserId" },
                unique: true);
        }
    }
}
