using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kind",
                schema: "identity",
                table: "RefreshTokens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                schema: "identity",
                table: "RefreshTokens");
        }
    }
}
