using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveClassMaterialFilesToTheirOwnTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassGroupMaterialFiles",
                columns: table => new
                {
                    ClassGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassGroupMaterialFiles", x => new { x.ClassGroupId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_ClassGroupMaterialFiles_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassGroupMaterialFiles_ClassGroups_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassGroupMaterialFiles_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroupMaterialFiles_DocumentId",
                table: "ClassGroupMaterialFiles",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroupMaterialFiles_TenantId",
                table: "ClassGroupMaterialFiles",
                column: "TenantId");

            migrationBuilder.Sql(
                """
                INSERT INTO [ClassGroupMaterialFiles] ([ClassGroupId], [DocumentId], [TenantId], [Position])
                SELECT [Id], [MaterialDocumentId], [TenantId], 0
                FROM [ClassGroups]
                WHERE [MaterialDocumentId] IS NOT NULL;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroups_Documents_MaterialDocumentId",
                table: "ClassGroups");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroups_MaterialDocumentId",
                table: "ClassGroups");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassGroups_OneMaterial",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "MaterialDocumentId",
                table: "ClassGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaterialDocumentId",
                table: "ClassGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroups_MaterialDocumentId",
                table: "ClassGroups",
                column: "MaterialDocumentId",
                unique: true,
                filter: "[MaterialDocumentId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassGroups_OneMaterial",
                table: "ClassGroups",
                sql: "[MaterialUrl] IS NULL OR [MaterialDocumentId] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroups_Documents_MaterialDocumentId",
                table: "ClassGroups",
                column: "MaterialDocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                """
                UPDATE [ClassGroups]
                SET [MaterialDocumentId] = [MaterialFiles].[DocumentId], [MaterialUrl] = NULL
                FROM [ClassGroups]
                INNER JOIN [ClassGroupMaterialFiles] AS [MaterialFiles] ON [MaterialFiles].[ClassGroupId] = [ClassGroups].[Id]
                WHERE [MaterialFiles].[Position] = 0;
                """);

            migrationBuilder.DropTable(
                name: "ClassGroupMaterialFiles");
        }
    }
}
