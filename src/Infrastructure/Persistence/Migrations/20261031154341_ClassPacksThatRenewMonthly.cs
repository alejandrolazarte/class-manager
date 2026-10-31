using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClassPacksThatRenewMonthly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClassCount",
                table: "ClassPacks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "AppliesFamilyDiscount",
                table: "ClassPacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RenewsMonthly",
                table: "ClassPacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassPacks_ClassCount_RenewsMonthly",
                table: "ClassPacks",
                sql: "[ClassCount] IS NOT NULL OR [RenewsMonthly] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassPacks_ClassCount_RenewsMonthly",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "AppliesFamilyDiscount",
                table: "ClassPacks");

            migrationBuilder.DropColumn(
                name: "RenewsMonthly",
                table: "ClassPacks");

            migrationBuilder.AlterColumn<int>(
                name: "ClassCount",
                table: "ClassPacks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
