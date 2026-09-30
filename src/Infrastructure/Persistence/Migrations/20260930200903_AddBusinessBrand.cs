using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccentColor",
                table: "Businesses",
                type: "nchar(7)",
                fixedLength: true,
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandName",
                table: "Businesses",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LocksTheme",
                table: "Businesses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LogoUpdatedAt",
                table: "Businesses",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThemeColor",
                table: "Businesses",
                type: "nchar(7)",
                fixedLength: true,
                maxLength: 7,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BrandLogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandLogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandLogos_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandLogos_TenantId",
                table: "BrandLogos",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandLogos");

            migrationBuilder.DropColumn(
                name: "AccentColor",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "BrandName",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "LocksTheme",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "LogoUpdatedAt",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "ThemeColor",
                table: "Businesses");
        }
    }
}
