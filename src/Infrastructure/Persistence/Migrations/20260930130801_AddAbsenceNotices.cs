using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAbsenceNotices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NoticedAbsencesKeepStreak",
                table: "Businesses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "AbsenceNotices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeepsStreak = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceNotices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbsenceNotices_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbsenceNotices_ClassSessions_ClassSessionId",
                        column: x => x.ClassSessionId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbsenceNotices_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceNotices_ClassSessionId",
                table: "AbsenceNotices",
                column: "ClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceNotices_StudentId",
                table: "AbsenceNotices",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceNotices_TenantId_ClassSessionId_StudentId",
                table: "AbsenceNotices",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceNotices_TenantId_StudentId",
                table: "AbsenceNotices",
                columns: new[] { "TenantId", "StudentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsenceNotices");

            migrationBuilder.DropColumn(
                name: "NoticedAbsencesKeepStreak",
                table: "Businesses");
        }
    }
}
