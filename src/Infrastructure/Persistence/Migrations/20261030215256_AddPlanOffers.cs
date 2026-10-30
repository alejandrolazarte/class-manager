using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanOffers",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriberId = table.Column<long>(type: "bigint", nullable: false),
                    PlanCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ExpiredOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanOffers", x => x.Id);
                    table.CheckConstraint("CK_PlanOffers_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_PlanOffers_Organizations_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanOffers_Plans_PlanCode",
                        column: x => x.PlanCode,
                        principalSchema: "billing",
                        principalTable: "Plans",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanOffers_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanOffers_DeletedByUserId",
                schema: "billing",
                table: "PlanOffers",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanOffers_PlanCode",
                schema: "billing",
                table: "PlanOffers",
                column: "PlanCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanOffers_SubscriberId",
                schema: "billing",
                table: "PlanOffers",
                column: "SubscriberId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanOffers",
                schema: "billing");
        }
    }
}
