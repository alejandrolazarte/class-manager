using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLegalTermsAcceptanceToPendingSignUps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AcceptedLegalTermsOn",
                table: "PendingSignUps",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedLegalTermsVersion",
                table: "PendingSignUps",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedLegalTermsOn",
                table: "PendingSignUps");

            migrationBuilder.DropColumn(
                name: "AcceptedLegalTermsVersion",
                table: "PendingSignUps");
        }
    }
}
