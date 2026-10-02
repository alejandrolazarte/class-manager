using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextOrderNumber",
                table: "Businesses",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                """
                WITH NumberedOrders AS (
                    SELECT [Number], ROW_NUMBER() OVER (PARTITION BY [TenantId] ORDER BY [CreatedAt], [Id]) AS [NewNumber]
                    FROM [Orders]
                )
                UPDATE NumberedOrders SET [Number] = [NewNumber];
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Businesses]
                SET [NextOrderNumber] = ISNULL((SELECT MAX([Number]) FROM [Orders] WHERE [Orders].[TenantId] = [Businesses].[Id]), 0) + 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TenantId_Number",
                table: "Orders",
                columns: new[] { "TenantId", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_TenantId_Number",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NextOrderNumber",
                table: "Businesses");
        }
    }
}
