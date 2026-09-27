using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeHistoryAndClassPacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassPacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ClassCount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ValidityMonths = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassPacks_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientBillingPlanChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CustomFee = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientBillingPlanChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientBillingPlanChanges_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientBillingPlanChanges_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefaultMonthlyFeeChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultMonthlyFeeChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultMonthlyFeeChanges_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassPackPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassPackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ClassCount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    PurchasedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpiresOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPackPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchases_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchases_ClassPacks_ClassPackId",
                        column: x => x.ClassPackId,
                        principalTable: "ClassPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPurchases_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_ClassPackId",
                table: "ClassPackPurchases",
                column: "ClassPackId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_ClientId",
                table: "ClassPackPurchases",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_ClientId",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_PurchasedOn",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "PurchasedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPacks_TenantId_Name",
                table: "ClassPacks",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_ClientId",
                table: "ClientBillingPlanChanges",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges",
                columns: new[] { "TenantId", "ClientId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges",
                columns: new[] { "TenantId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO DefaultMonthlyFeeChanges (Id, TenantId, EffectiveFrom, Amount, CreatedAt)
                SELECT NEWID(), Id, '2000-01-01', DefaultMonthlyFee, SYSUTCDATETIME()
                FROM Businesses
                WHERE DefaultMonthlyFee IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                INSERT INTO ClientBillingPlanChanges (Id, TenantId, ClientId, EffectiveFrom, Kind, CustomFee, CreatedAt)
                SELECT NEWID(), TenantId, Id, '2000-01-01', 'CustomFee', MonthlyFee, SYSUTCDATETIME()
                FROM Clients
                WHERE MonthlyFee IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "MonthlyFee",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DefaultMonthlyFee",
                table: "Businesses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyFee",
                table: "Clients",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMonthlyFee",
                table: "Businesses",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Businesses
                SET DefaultMonthlyFee = (
                    SELECT TOP 1 Amount FROM DefaultMonthlyFeeChanges
                    WHERE DefaultMonthlyFeeChanges.TenantId = Businesses.Id AND EffectiveFrom <= SYSUTCDATETIME()
                    ORDER BY EffectiveFrom DESC);
                """);

            migrationBuilder.Sql("""
                UPDATE Clients
                SET MonthlyFee = (
                    SELECT TOP 1 CustomFee FROM ClientBillingPlanChanges
                    WHERE ClientBillingPlanChanges.ClientId = Clients.Id AND EffectiveFrom <= SYSUTCDATETIME()
                    ORDER BY EffectiveFrom DESC);
                """);

            migrationBuilder.DropTable(
                name: "ClassPackPurchases");

            migrationBuilder.DropTable(
                name: "ClientBillingPlanChanges");

            migrationBuilder.DropTable(
                name: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropTable(
                name: "ClassPacks");

        }
    }
}
