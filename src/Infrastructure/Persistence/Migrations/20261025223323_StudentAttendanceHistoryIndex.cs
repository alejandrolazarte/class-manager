using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StudentAttendanceHistoryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TenantId_StudentId",
                table: "Attendances",
                columns: new[] { "TenantId", "StudentId" })
                .Annotation("SqlServer:Include", new[] { "ClassSessionId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_TenantId_StudentId",
                table: "Attendances");
        }
    }
}
