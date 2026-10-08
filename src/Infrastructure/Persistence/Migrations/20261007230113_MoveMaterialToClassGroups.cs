using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveMaterialToClassGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialUrl",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "MaterialUrl",
                table: "ClassPackPurchases");

            migrationBuilder.AddColumn<string>(
                name: "MaterialUrl",
                table: "ClassGroups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialUrl",
                table: "ClassGroups");

            migrationBuilder.AddColumn<string>(
                name: "MaterialUrl",
                table: "ClassPacks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialUrl",
                table: "ClassPackPurchases",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
