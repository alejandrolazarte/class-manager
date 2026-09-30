using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClassFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassFeedbacks_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassFeedbacks_ClassSessions_ClassSessionId",
                        column: x => x.ClassSessionId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassFeedbacks_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassFeedbacks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_ClassSessionId",
                table: "ClassFeedbacks",
                column: "ClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_InstructorId",
                table: "ClassFeedbacks",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_StudentId",
                table: "ClassFeedbacks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_TenantId_StudentId",
                table: "ClassFeedbacks",
                columns: new[] { "TenantId", "StudentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassFeedbacks");
        }
    }
}
