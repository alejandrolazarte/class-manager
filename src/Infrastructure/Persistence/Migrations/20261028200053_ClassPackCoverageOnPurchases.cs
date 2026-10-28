using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClassPackCoverageOnPurchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CoversAnyGroupClass",
                table: "ClassPacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoversAnyGroupClass",
                table: "ClassPackPurchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ClassPackPurchaseClassGroups",
                columns: table => new
                {
                    ClassPackPurchaseId = table.Column<long>(type: "bigint", nullable: false),
                    ClassGroupId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPackPurchaseClassGroups", x => new { x.ClassPackPurchaseId, x.ClassGroupId });
                    table.ForeignKey(
                        name: "FK_ClassPackPurchaseClassGroups_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchaseClassGroups_ClassGroups_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchaseClassGroups_ClassPackPurchases_ClassPackPurchaseId",
                        column: x => x.ClassPackPurchaseId,
                        principalTable: "ClassPackPurchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchaseClassGroups_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchaseClassGroups_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchaseClassGroups_ClassGroupId",
                table: "ClassPackPurchaseClassGroups",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchaseClassGroups_CreatedByUserId",
                table: "ClassPackPurchaseClassGroups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchaseClassGroups_TenantId_ClassGroupId",
                table: "ClassPackPurchaseClassGroups",
                columns: new[] { "TenantId", "ClassGroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchaseClassGroups_UpdatedByUserId",
                table: "ClassPackPurchaseClassGroups",
                column: "UpdatedByUserId");

            migrationBuilder.Sql("""
                INSERT INTO [ClassPackPurchaseClassGroups] ([ClassPackPurchaseId], [ClassGroupId], [TenantId], [CreatedAt], [UpdatedAt])
                SELECT [purchase].[Id], [packClassGroup].[ClassGroupId], [purchase].[TenantId], [purchase].[CreatedAt], [purchase].[CreatedAt]
                FROM [ClassPackPurchases] AS [purchase]
                INNER JOIN [ClassPackClassGroups] AS [packClassGroup] ON [packClassGroup].[ClassPackId] = [purchase].[ClassPackId]
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassPackPurchaseClassGroups");

            migrationBuilder.DropColumn(
                name: "CoversAnyGroupClass",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "CoversAnyGroupClass",
                table: "ClassPackPurchases");
        }
    }
}
