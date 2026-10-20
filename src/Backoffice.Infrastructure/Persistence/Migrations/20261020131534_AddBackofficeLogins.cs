using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBackofficeLogins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "backoffice");

            migrationBuilder.CreateTable(
                name: "BackofficeAccessRequests",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackofficeAccessRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackofficeLogins",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EnabledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EnabledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackofficeLogins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackofficeAccessRequests_Provider_ProviderKey",
                schema: "backoffice",
                table: "BackofficeAccessRequests",
                columns: new[] { "Provider", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackofficeLogins_AccountId",
                schema: "backoffice",
                table: "BackofficeLogins",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BackofficeLogins_Provider_ProviderKey",
                schema: "backoffice",
                table: "BackofficeLogins",
                columns: new[] { "Provider", "ProviderKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackofficeAccessRequests",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "BackofficeLogins",
                schema: "backoffice");
        }
    }
}
