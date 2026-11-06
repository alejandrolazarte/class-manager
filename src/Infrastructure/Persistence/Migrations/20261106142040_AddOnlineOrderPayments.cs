using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOnlineOrderPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalPaymentId",
                table: "OrderPayments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_ExternalPaymentId",
                table: "OrderPayments",
                column: "ExternalPaymentId",
                unique: true,
                filter: "[IsDeleted] = 0 AND [ExternalPaymentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderPayments_ExternalPaymentId",
                table: "OrderPayments");

            migrationBuilder.DropColumn(
                name: "ExternalPaymentId",
                table: "OrderPayments");
        }
    }
}
