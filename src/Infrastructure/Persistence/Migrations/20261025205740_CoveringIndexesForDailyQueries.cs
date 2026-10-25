using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CoveringIndexesForDailyQueries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_ClientId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TenantId_Month_ClientId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TenantId_RegisteredByUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_TenantId_ClassSessionId_StudentId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_ClientId",
                table: "Students",
                columns: new[] { "TenantId", "ClientId" })
                .Annotation("SqlServer:Include", new[] { "UserId", "Notes" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TenantId_Month_ClientId",
                table: "Payments",
                columns: new[] { "TenantId", "Month", "ClientId" })
                .Annotation("SqlServer:Include", new[] { "Amount", "CurrencyCode", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TenantId_StartDate",
                table: "Enrollments",
                columns: new[] { "TenantId", "StartDate" })
                .Annotation("SqlServer:Include", new[] { "EndDate", "ClassGroupId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TenantId_RegisteredByUserId",
                table: "Clients",
                columns: new[] { "TenantId", "RegisteredByUserId" })
                .Annotation("SqlServer:Include", new[] { "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TenantId_ClassSessionId_StudentId",
                table: "Attendances",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true)
                .Annotation("SqlServer:Include", new[] { "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_ClientId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TenantId_Month_ClientId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_TenantId_StartDate",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TenantId_RegisteredByUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_TenantId_ClassSessionId_StudentId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_ClientId",
                table: "Students",
                columns: new[] { "TenantId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TenantId_Month_ClientId",
                table: "Payments",
                columns: new[] { "TenantId", "Month", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TenantId_RegisteredByUserId",
                table: "Clients",
                columns: new[] { "TenantId", "RegisteredByUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TenantId_ClassSessionId_StudentId",
                table: "Attendances",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true);
        }
    }
}
