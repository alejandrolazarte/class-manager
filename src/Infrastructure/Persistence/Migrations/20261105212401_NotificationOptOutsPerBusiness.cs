using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NotificationOptOutsPerBusiness : Migration
    {
        private const string CopyChoicesToEveryBusinessOfTheAccount = """
            INSERT INTO [NotificationOptOuts] ([TenantId], [AccountId], [Category], [Channel], [CreatedAt], [UpdatedAt])
            SELECT DISTINCT [Member].[TenantId], [OptOut].[AccountId], [OptOut].[Category], [OptOut].[Channel], [OptOut].[CreatedAt], [OptOut].[CreatedAt]
            FROM [NotificationOptOuts] AS [OptOut]
            JOIN [Users] AS [User] ON [User].[AccountId] = [OptOut].[AccountId]
            JOIN [BusinessMembers] AS [Member] ON [Member].[UserId] = [User].[Id] AND [Member].[IsDeleted] = 0
            WHERE [OptOut].[TenantId] IS NULL AND [OptOut].[Category] IN ('NewOrders', 'Attendance', 'StudentPlans', 'Invitations');

            INSERT INTO [NotificationOptOuts] ([TenantId], [AccountId], [Category], [Channel], [CreatedAt], [UpdatedAt])
            SELECT DISTINCT [Access].[TenantId], [OptOut].[AccountId], [OptOut].[Category], [OptOut].[Channel], [OptOut].[CreatedAt], [OptOut].[CreatedAt]
            FROM [NotificationOptOuts] AS [OptOut]
            JOIN [Users] AS [User] ON [User].[AccountId] = [OptOut].[AccountId]
            JOIN [ClientAccounts] AS [Access] ON [Access].[UserId] = [User].[Id] AND [Access].[IsDeleted] = 0
            WHERE [OptOut].[TenantId] IS NULL AND [OptOut].[Category] IN ('Orders', 'News', 'ClassChanges', 'InstructorComments');
            """;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationOptOuts",
                table: "NotificationOptOuts");

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                table: "NotificationOptOuts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "NotificationOptOuts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "NotificationOptOuts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "NotificationOptOuts",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql(CopyChoicesToEveryBusinessOfTheAccount);
            migrationBuilder.Sql("DELETE FROM [NotificationOptOuts] WHERE [TenantId] IS NULL;");

            migrationBuilder.AlterColumn<long>(
                name: "TenantId",
                table: "NotificationOptOuts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationOptOuts",
                table: "NotificationOptOuts",
                columns: new[] { "TenantId", "AccountId", "Category", "Channel" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationOptOuts_CreatedByUserId",
                table: "NotificationOptOuts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationOptOuts_UpdatedByUserId",
                table: "NotificationOptOuts",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationOptOuts_Businesses_TenantId",
                table: "NotificationOptOuts",
                column: "TenantId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationOptOuts_Users_CreatedByUserId",
                table: "NotificationOptOuts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationOptOuts_Users_UpdatedByUserId",
                table: "NotificationOptOuts",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationOptOuts_Businesses_TenantId",
                table: "NotificationOptOuts");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationOptOuts_Users_CreatedByUserId",
                table: "NotificationOptOuts");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationOptOuts_Users_UpdatedByUserId",
                table: "NotificationOptOuts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationOptOuts",
                table: "NotificationOptOuts");

            migrationBuilder.Sql("DELETE FROM [NotificationOptOuts];");

            migrationBuilder.DropIndex(
                name: "IX_NotificationOptOuts_CreatedByUserId",
                table: "NotificationOptOuts");

            migrationBuilder.DropIndex(
                name: "IX_NotificationOptOuts_UpdatedByUserId",
                table: "NotificationOptOuts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "NotificationOptOuts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "NotificationOptOuts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "NotificationOptOuts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "NotificationOptOuts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationOptOuts",
                table: "NotificationOptOuts",
                columns: new[] { "AccountId", "Category", "Channel" });
        }
    }
}
