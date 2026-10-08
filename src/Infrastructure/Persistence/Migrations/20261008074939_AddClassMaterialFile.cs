using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClassMaterialFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
