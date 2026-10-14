using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementAudienceAndPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Announcements",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Announcements",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "News");

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "VisibleUntil",
                table: "Announcements",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnnouncementClassGroups",
                columns: table => new
                {
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementClassGroups", x => new { x.AnnouncementId, x.ClassGroupId });
                    table.ForeignKey(
                        name: "FK_AnnouncementClassGroups_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementClassGroups_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnouncementClassGroups_ClassGroups_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementImages",
                columns: table => new
                {
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementImages", x => new { x.AnnouncementId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_AnnouncementImages_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementImages_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnouncementImages_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_TenantId_Pinned",
                table: "Announcements",
                column: "TenantId",
                unique: true,
                filter: "[IsPinned] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementClassGroups_ClassGroupId",
                table: "AnnouncementClassGroups",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementClassGroups_TenantId_ClassGroupId",
                table: "AnnouncementClassGroups",
                columns: new[] { "TenantId", "ClassGroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementImages_DocumentId",
                table: "AnnouncementImages",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementImages_TenantId",
                table: "AnnouncementImages",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementClassGroups");

            migrationBuilder.DropTable(
                name: "AnnouncementImages");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_TenantId_Pinned",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "VisibleUntil",
                table: "Announcements");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Announcements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
