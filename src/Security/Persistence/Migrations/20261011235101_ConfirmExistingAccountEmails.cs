using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfirmExistingAccountEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [identity].[AspNetUsers] SET [EmailConfirmed] = CAST(1 AS bit) WHERE [EmailConfirmed] = CAST(0 AS bit);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
