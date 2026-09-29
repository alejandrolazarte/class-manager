using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RegisteredByUserId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId",
                table: "BusinessMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TenantId_RegisteredByUserId",
                table: "Clients",
                columns: new[] { "TenantId", "RegisteredByUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_InstructorId",
                table: "BusinessMembers",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_InstructorId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "InstructorId" },
                unique: true,
                filter: "[InstructorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMembers_Instructors_InstructorId",
                table: "BusinessMembers",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMembers_Instructors_InstructorId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TenantId_RegisteredByUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_InstructorId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_InstructorId",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "RegisteredByUserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "BusinessMembers");
        }
    }
}
