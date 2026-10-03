using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClassPackClassGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassPackClassGroups",
                columns: table => new
                {
                    ClassPackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPackClassGroups", x => new { x.ClassPackId, x.ClassGroupId });
                    table.ForeignKey(
                        name: "FK_ClassPackClassGroups_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackClassGroups_ClassGroups_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackClassGroups_ClassPacks_ClassPackId",
                        column: x => x.ClassPackId,
                        principalTable: "ClassPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackClassGroups_ClassGroupId",
                table: "ClassPackClassGroups",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackClassGroups_TenantId_ClassGroupId",
                table: "ClassPackClassGroups",
                columns: new[] { "TenantId", "ClassGroupId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassPackClassGroups");
        }
    }
}
