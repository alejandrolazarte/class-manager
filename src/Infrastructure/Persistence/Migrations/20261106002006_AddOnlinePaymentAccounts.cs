using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOnlinePaymentAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OnlinePaymentAccountId",
                table: "Businesses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OnlinePaymentAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedForBusinessId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalAccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DetailsSubmitted = table.Column<bool>(type: "bit", nullable: false),
                    ChargesEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PayoutsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StatusReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlinePaymentAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentAccounts_Businesses_CreatedForBusinessId",
                        column: x => x.CreatedForBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentAccounts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_OnlinePaymentAccountId",
                table: "Businesses",
                column: "OnlinePaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentAccounts_CreatedForBusinessId",
                table: "OnlinePaymentAccounts",
                column: "CreatedForBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentAccounts_ExternalAccountId",
                table: "OnlinePaymentAccounts",
                column: "ExternalAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentAccounts_OrganizationId",
                table: "OnlinePaymentAccounts",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_OnlinePaymentAccounts_OnlinePaymentAccountId",
                table: "Businesses",
                column: "OnlinePaymentAccountId",
                principalTable: "OnlinePaymentAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_OnlinePaymentAccounts_OnlinePaymentAccountId",
                table: "Businesses");

            migrationBuilder.DropTable(
                name: "OnlinePaymentAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_OnlinePaymentAccountId",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "OnlinePaymentAccountId",
                table: "Businesses");
        }
    }
}
