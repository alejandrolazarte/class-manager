using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "billing",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsCounted = table.Column<bool>(type: "bit", nullable: false),
                    IsAddOn = table.Column<bool>(type: "bit", nullable: false),
                    AddOnListPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "billing",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    BillingPeriod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PlanFeatures",
                schema: "billing",
                columns: table => new
                {
                    PlanCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FeatureCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Limit = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanFeatures", x => new { x.PlanCode, x.FeatureCode });
                    table.ForeignKey(
                        name: "FK_PlanFeatures_Features_FeatureCode",
                        column: x => x.FeatureCode,
                        principalSchema: "billing",
                        principalTable: "Features",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanFeatures_Plans_PlanCode",
                        column: x => x.PlanCode,
                        principalSchema: "billing",
                        principalTable: "Plans",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    StartsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Organizations_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Plans_PlanCode",
                        column: x => x.PlanCode,
                        principalSchema: "billing",
                        principalTable: "Plans",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionFeatures",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Limit = table.Column<int>(type: "int", nullable: true),
                    StartsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionFeatures_Features_FeatureCode",
                        column: x => x.FeatureCode,
                        principalSchema: "billing",
                        principalTable: "Features",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionFeatures_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "billing",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "Features",
                columns: new[] { "Code", "AddOnListPrice", "Currency", "IsAddOn", "IsCounted" },
                values: new object[,]
                {
                    { "branches", 10m, "USD", true, true },
                    { "brand", 5m, "USD", true, false },
                    { "class-packs", null, null, false, false },
                    { "custom-roles", null, null, false, false },
                    { "family-app", 5m, "USD", true, false },
                    { "import-export", 3m, "USD", true, false },
                    { "shop", 5m, "USD", true, false },
                    { "students", null, null, false, true },
                    { "team", null, null, false, true }
                });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "Plans",
                columns: new[] { "Code", "BillingPeriod", "Currency", "DisplayOrder", "IsActive", "IsDefault", "ListPrice" },
                values: new object[,]
                {
                    { "enterprise", "Monthly", "USD", 4, true, false, null },
                    { "free", "Monthly", "USD", 1, true, true, 0m },
                    { "lite", "Monthly", "USD", 2, true, false, 9m },
                    { "pro", "Monthly", "USD", 3, true, false, 19m }
                });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[,]
                {
                    { "branches", "enterprise", null },
                    { "brand", "enterprise", null },
                    { "class-packs", "enterprise", null },
                    { "custom-roles", "enterprise", null },
                    { "family-app", "enterprise", null },
                    { "import-export", "enterprise", null },
                    { "shop", "enterprise", null },
                    { "students", "enterprise", null },
                    { "team", "enterprise", null },
                    { "branches", "free", 1 },
                    { "students", "free", 30 },
                    { "team", "free", 0 },
                    { "branches", "lite", 1 },
                    { "class-packs", "lite", null },
                    { "import-export", "lite", null },
                    { "students", "lite", 150 },
                    { "team", "lite", 2 },
                    { "branches", "pro", 3 },
                    { "brand", "pro", null },
                    { "class-packs", "pro", null },
                    { "custom-roles", "pro", null },
                    { "family-app", "pro", null },
                    { "import-export", "pro", null },
                    { "shop", "pro", null },
                    { "students", "pro", null },
                    { "team", "pro", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_FeatureCode",
                schema: "billing",
                table: "PlanFeatures",
                column: "FeatureCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatures_FeatureCode",
                schema: "billing",
                table: "SubscriptionFeatures",
                column: "FeatureCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId",
                schema: "billing",
                table: "SubscriptionFeatures",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanCode",
                schema: "billing",
                table: "Subscriptions",
                column: "PlanCode");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions",
                column: "SubscriberId",
                unique: true,
                filter: "[EndsOn] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId_StartsOn",
                schema: "billing",
                table: "Subscriptions",
                columns: new[] { "SubscriberId", "StartsOn" });

            migrationBuilder.Sql("""
                INSERT INTO billing.Subscriptions (Id, SubscriberId, PlanCode, Price, Currency, StartsOn, EndsOn, Note, CreatedAt)
                SELECT NEWID(), Id, 'enterprise', 0, 'USD', CAST(CreatedAt AS date), NULL, 'Pilot', SYSUTCDATETIME()
                FROM Organizations;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanFeatures",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "SubscriptionFeatures",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "billing");
        }
    }
}
