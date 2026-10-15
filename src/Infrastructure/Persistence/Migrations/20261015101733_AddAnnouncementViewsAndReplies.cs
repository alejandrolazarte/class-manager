using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementViewsAndReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnouncementReplies",
                columns: table => new
                {
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGoing = table.Column<bool>(type: "bit", nullable: false),
                    RepliedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementReplies", x => new { x.AnnouncementId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_AnnouncementReplies_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementReplies_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnouncementReplies_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementViews",
                columns: table => new
                {
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementViews", x => new { x.AnnouncementId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_AnnouncementViews_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementViews_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnouncementViews_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementReplies_ClientId",
                table: "AnnouncementReplies",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementReplies_TenantId_ClientId",
                table: "AnnouncementReplies",
                columns: new[] { "TenantId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementViews_ClientId",
                table: "AnnouncementViews",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementViews_TenantId_ClientId",
                table: "AnnouncementViews",
                columns: new[] { "TenantId", "ClientId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementReplies");

            migrationBuilder.DropTable(
                name: "AnnouncementViews");
        }
    }
}
