using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MonthlyPlansReplaceFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassPackPriceChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    ClassPackId = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPackPriceChanges", x => x.Id);
                    table.CheckConstraint("CK_ClassPackPriceChanges_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ClassPackPriceChanges_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPriceChanges_ClassPacks_ClassPackId",
                        column: x => x.ClassPackId,
                        principalTable: "ClassPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPriceChanges_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPriceChanges_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPackPriceChanges_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentPlanChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ClassPackId = table.Column<long>(type: "bigint", nullable: true),
                    CustomPrice = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPlanChanges", x => x.Id);
                    table.CheckConstraint("CK_StudentPlanChanges_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_StudentPlanChanges_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentPlanChanges_ClassPacks_ClassPackId",
                        column: x => x.ClassPackId,
                        principalTable: "ClassPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentPlanChanges_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentPlanChanges_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentPlanChanges_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentPlanChanges_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPriceChanges_ClassPackId",
                table: "ClassPackPriceChanges",
                column: "ClassPackId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPriceChanges_CreatedByUserId",
                table: "ClassPackPriceChanges",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPriceChanges_DeletedByUserId",
                table: "ClassPackPriceChanges",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPriceChanges_TenantId_ClassPackId_EffectiveFrom",
                table: "ClassPackPriceChanges",
                columns: new[] { "TenantId", "ClassPackId", "EffectiveFrom" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPriceChanges_UpdatedByUserId",
                table: "ClassPackPriceChanges",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPlanChanges_ClassPackId",
                table: "StudentPlanChanges",
                column: "ClassPackId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPlanChanges_CreatedByUserId",
                table: "StudentPlanChanges",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPlanChanges_DeletedByUserId",
                table: "StudentPlanChanges",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPlanChanges_StudentId",
                table: "StudentPlanChanges",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPlanChanges_TenantId_StudentId_EffectiveFrom",
                table: "StudentPlanChanges",
                columns: new[] { "TenantId", "StudentId", "EffectiveFrom" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPlanChanges_UpdatedByUserId",
                table: "StudentPlanChanges",
                column: "UpdatedByUserId");

            migrationBuilder.Sql("""
                DECLARE @now datetimeoffset = SYSDATETIMEOFFSET();
                DECLARE @plans TABLE ([TenantId] bigint NOT NULL, [ClassPackId] bigint NOT NULL);

                INSERT INTO [ClassPacks] ([TenantId], [Name], [Description], [ClassCount], [Price], [ValidityMonths], [ClassDurationMinutes], [CoversAnyGroupClass], [IsActive], [RenewsMonthly], [AppliesFamilyDiscount], [CreatedAt], [UpdatedAt])
                OUTPUT [inserted].[TenantId], [inserted].[Id] INTO @plans ([TenantId], [ClassPackId])
                SELECT
                    [tenant].[TenantId],
                    CASE WHEN EXISTS (SELECT 1 FROM [ClassPacks] AS [pack] WHERE [pack].[TenantId] = [tenant].[TenantId] AND [pack].[Name] = N'Cuota mensual')
                        THEN N'Cuota mensual (plan)'
                        ELSE N'Cuota mensual'
                    END,
                    NULL,
                    NULL,
                    ISNULL((
                        SELECT TOP 1 [fee].[Amount]
                        FROM [DefaultMonthlyFeeChanges] AS [fee]
                        WHERE [fee].[TenantId] = [tenant].[TenantId] AND [fee].[IsDeleted] = 0
                        ORDER BY [fee].[EffectiveFrom]), 0),
                    NULL,
                    NULL,
                    1,
                    1,
                    1,
                    1,
                    @now,
                    @now
                FROM (
                    SELECT DISTINCT [TenantId]
                    FROM [ClientBillingPlanChanges]
                    WHERE [IsDeleted] = 0 AND [Kind] IN (N'BusinessFee', N'CustomFee')
                ) AS [tenant];

                INSERT INTO [ClassPackPriceChanges] ([TenantId], [ClassPackId], [EffectiveFrom], [Price], [CreatedAt], [UpdatedAt], [IsDeleted])
                SELECT [fee].[TenantId], [plan].[ClassPackId], [fee].[EffectiveFrom], ISNULL([fee].[Amount], 0), @now, @now, 0
                FROM [DefaultMonthlyFeeChanges] AS [fee]
                INNER JOIN @plans AS [plan] ON [plan].[TenantId] = [fee].[TenantId]
                WHERE [fee].[IsDeleted] = 0;

                INSERT INTO [StudentPlanChanges] ([TenantId], [StudentId], [EffectiveFrom], [ClassPackId], [CustomPrice], [CreatedAt], [UpdatedAt], [IsDeleted])
                SELECT
                    [change].[TenantId],
                    [student].[Id],
                    [change].[EffectiveFrom],
                    CASE WHEN [change].[Kind] = N'ClassPacks' THEN NULL ELSE [plan].[ClassPackId] END,
                    CASE
                        WHEN [change].[Kind] = N'ClassPacks' THEN NULL
                        WHEN [student].[Position] > 1 THEN 0
                        WHEN [change].[Kind] = N'CustomFee' THEN [change].[CustomFee]
                        ELSE NULL
                    END,
                    @now,
                    @now,
                    0
                FROM [ClientBillingPlanChanges] AS [change]
                INNER JOIN (
                    SELECT [Id], [ClientId], ROW_NUMBER() OVER (PARTITION BY [ClientId] ORDER BY [IsDeleted], [Id]) AS [Position]
                    FROM [Students]
                ) AS [student] ON [student].[ClientId] = [change].[ClientId]
                LEFT JOIN @plans AS [plan] ON [plan].[TenantId] = [change].[TenantId]
                WHERE [change].[IsDeleted] = 0;
                """);

            migrationBuilder.DropTable(
                name: "ClientBillingPlanChanges");

            migrationBuilder.DropTable(
                name: "DefaultMonthlyFeeChanges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassPackPriceChanges");

            migrationBuilder.DropTable(
                name: "StudentPlanChanges");

            migrationBuilder.CreateTable(
                name: "ClientBillingPlanChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CustomFee = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientBillingPlanChanges", x => x.Id);
                    table.CheckConstraint("CK_ClientBillingPlanChanges_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
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
                    table.ForeignKey(
                        name: "FK_ClientBillingPlanChanges_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientBillingPlanChanges_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientBillingPlanChanges_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefaultMonthlyFeeChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    BillingMode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultMonthlyFeeChanges", x => x.Id);
                    table.CheckConstraint("CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_DefaultMonthlyFeeChanges_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultMonthlyFeeChanges_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultMonthlyFeeChanges_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultMonthlyFeeChanges_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_ClientId",
                table: "ClientBillingPlanChanges",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_CreatedByUserId",
                table: "ClientBillingPlanChanges",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_DeletedByUserId",
                table: "ClientBillingPlanChanges",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges",
                columns: new[] { "TenantId", "ClientId", "EffectiveFrom" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_UpdatedByUserId",
                table: "ClientBillingPlanChanges",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_CreatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_DeletedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges",
                columns: new[] { "TenantId", "EffectiveFrom" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_UpdatedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "UpdatedByUserId");
        }
    }
}
