using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalesFeePercent",
                schema: "billing",
                table: "Plans",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SalesFeeOverrides",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriberId = table.Column<long>(type: "bigint", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesFeeOverrides", x => x.Id);
                    table.CheckConstraint("CK_SalesFeeOverrides_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SalesFeeOverrides_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "enterprise",
                column: "SalesFeePercent",
                value: 0m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "free",
                column: "SalesFeePercent",
                value: 0m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "lite",
                column: "SalesFeePercent",
                value: 0m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "pro",
                column: "SalesFeePercent",
                value: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_SalesFeeOverrides_DeletedByUserId",
                schema: "billing",
                table: "SalesFeeOverrides",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesFeeOverrides_SubscriberId",
                schema: "billing",
                table: "SalesFeeOverrides",
                column: "SubscriberId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesFeeOverrides",
                schema: "billing");

            migrationBuilder.DropColumn(
                name: "SalesFeePercent",
                schema: "billing",
                table: "Plans");
        }
    }
}
