using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentEmailAndPersonalAppAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "ClientInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "ClientAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvitations_StudentId",
                table: "ClientInvitations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_StudentId",
                table: "ClientAccounts",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAccounts_Students_StudentId",
                table: "ClientAccounts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvitations_Students_StudentId",
                table: "ClientInvitations",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientAccounts_Students_StudentId",
                table: "ClientAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvitations_Students_StudentId",
                table: "ClientInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvitations_StudentId",
                table: "ClientInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_StudentId",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "ClientInvitations");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "ClientAccounts");
        }
    }
}
