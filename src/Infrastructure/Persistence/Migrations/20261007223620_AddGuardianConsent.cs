using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianConsent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "GuardianConsentedAt",
                table: "ClientInvitations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianEmail",
                table: "ClientInvitations",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianTokenHash",
                table: "ClientInvitations",
                type: "nchar(64)",
                fixedLength: true,
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvitations_GuardianTokenHash",
                table: "ClientInvitations",
                column: "GuardianTokenHash",
                unique: true,
                filter: "[GuardianTokenHash] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientInvitations_GuardianTokenHash",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "GuardianConsentedAt",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "GuardianEmail",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "GuardianTokenHash",
                table: "ClientInvitations");
        }
    }
}
