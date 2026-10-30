using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSelfServiceBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingCustomers",
                schema: "billing",
                columns: table => new
                {
                    SubscriberId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalCustomerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingCustomers", x => x.SubscriberId);
                    table.ForeignKey(
                        name: "FK_BillingCustomers_Organizations_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "Features",
                columns: new[] { "Code", "AddOnListPrice", "Currency", "IsAddOn", "IsCounted" },
                values: new object[] { "self-service-billing", 0m, "EUR", true, false });

            migrationBuilder.CreateIndex(
                name: "IX_BillingCustomers_ExternalCustomerId",
                schema: "billing",
                table: "BillingCustomers",
                column: "ExternalCustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingCustomers",
                schema: "billing");

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "self-service-billing");
        }
    }
}
