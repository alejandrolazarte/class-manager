using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomRoleId",
                table: "MemberInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomRoleId",
                table: "BusinessMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CopiedFrom = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomRoles_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitations_CustomRoleId",
                table: "MemberInvitations",
                column: "CustomRoleId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MemberInvitations_CustomRoleId",
                table: "MemberInvitations",
                sql: "([Role] = 'Custom' AND [CustomRoleId] IS NOT NULL) OR ([Role] <> 'Custom' AND [CustomRoleId] IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_CustomRoleId",
                table: "BusinessMembers",
                column: "CustomRoleId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BusinessMembers_CustomRoleId",
                table: "BusinessMembers",
                sql: "([Role] = 'Custom' AND [CustomRoleId] IS NOT NULL) OR ([Role] <> 'Custom' AND [CustomRoleId] IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_TenantId_Name",
                table: "CustomRoles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMembers_CustomRoles_CustomRoleId",
                table: "BusinessMembers",
                column: "CustomRoleId",
                principalTable: "CustomRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInvitations_CustomRoles_CustomRoleId",
                table: "MemberInvitations",
                column: "CustomRoleId",
                principalTable: "CustomRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMembers_CustomRoles_CustomRoleId",
                table: "BusinessMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInvitations_CustomRoles_CustomRoleId",
                table: "MemberInvitations");

            migrationBuilder.DropTable(
                name: "CustomRoles");

            migrationBuilder.DropIndex(
                name: "IX_MemberInvitations_CustomRoleId",
                table: "MemberInvitations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MemberInvitations_CustomRoleId",
                table: "MemberInvitations");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_CustomRoleId",
                table: "BusinessMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BusinessMembers_CustomRoleId",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "CustomRoleId",
                table: "MemberInvitations");

            migrationBuilder.DropColumn(
                name: "CustomRoleId",
                table: "BusinessMembers");
        }
    }
}
