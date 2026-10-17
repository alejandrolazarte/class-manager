using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContactAdultConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ContactAdultConfirmedAt",
                table: "ClientInvitations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactAdultConfirmedByUserId",
                table: "ClientInvitations",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAdultConfirmedAt",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "ContactAdultConfirmedByUserId",
                table: "ClientInvitations");
        }
    }
}
