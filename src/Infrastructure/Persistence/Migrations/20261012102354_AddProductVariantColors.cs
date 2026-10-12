using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductVariants",
                newName: "Size");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ProductVariants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderLines",
                type: "nvarchar(116)",
                maxLength: 116,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(93)",
                oldMaxLength: 93);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "ProductVariants");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "ProductVariants",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderLines",
                type: "nvarchar(93)",
                maxLength: 93,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(116)",
                oldMaxLength: 116);
        }
    }
}
