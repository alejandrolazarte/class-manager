using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ImprovesWhen = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsVisibleToStudents = table.Column<bool>(type: "bit", nullable: false),
                    IsRecordedByStudents = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDefinitions", x => x.Id);
                    table.CheckConstraint("CK_MeasurementDefinitions_IsDeleted_DeletedOn", "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_MeasurementDefinitions_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeasurementDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementGoals_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementGoals_MeasurementDefinitions_MeasurementDefinitionId",
                        column: x => x.MeasurementDefinitionId,
                        principalTable: "MeasurementDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementGoals_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeasurementDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeasuredOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecordedByStudent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementRecords", x => x.Id);
                    table.CheckConstraint("CK_MeasurementRecords_IsDeleted_DeletedOn", "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_MeasurementRecords_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementRecords_MeasurementDefinitions_MeasurementDefinitionId",
                        column: x => x.MeasurementDefinitionId,
                        principalTable: "MeasurementDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitions_TenantId_Name",
                table: "MeasurementDefinitions",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementGoals_MeasurementDefinitionId",
                table: "MeasurementGoals",
                column: "MeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementGoals_StudentId",
                table: "MeasurementGoals",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementGoals_TenantId_StudentId_MeasurementDefinitionId",
                table: "MeasurementGoals",
                columns: new[] { "TenantId", "StudentId", "MeasurementDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_MeasurementDefinitionId",
                table: "MeasurementRecords",
                column: "MeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_StudentId",
                table: "MeasurementRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_TenantId_StudentId_MeasurementDefinitionId_MeasuredOn",
                table: "MeasurementRecords",
                columns: new[] { "TenantId", "StudentId", "MeasurementDefinitionId", "MeasuredOn" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementGoals");

            migrationBuilder.DropTable(
                name: "MeasurementRecords");

            migrationBuilder.DropTable(
                name: "MeasurementDefinitions");
        }
    }
}
