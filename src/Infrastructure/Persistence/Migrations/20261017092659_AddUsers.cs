using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_TenantId_UserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_UserId",
                table: "BusinessMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Instructors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_UserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_UserId",
                table: "BusinessMembers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ClientAccounts",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BusinessMembers",
                newName: "AccountId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ClientAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "BusinessMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_Users_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Users_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(UsersBackfill.Clients);
            migrationBuilder.Sql(UsersBackfill.AttendingContacts);
            migrationBuilder.Sql(UsersBackfill.Students);
            migrationBuilder.Sql(UsersBackfill.Instructors);
            migrationBuilder.Sql(UsersBackfill.MembersWhoTeach);
            migrationBuilder.Sql(UsersBackfill.ClientAccounts);
            migrationBuilder.Sql(UsersBackfill.AccountsOfMembersWhoTeach);
            migrationBuilder.Sql(UsersBackfill.MembersOfLinkedAccounts);
            migrationBuilder.Sql(UsersBackfill.OtherMembers);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_UserId",
                table: "Instructors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_UserId",
                table: "ClientAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_UserId",
                table: "BusinessMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_AccountId",
                table: "ClientAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_TenantId_AccountId",
                table: "ClientAccounts",
                columns: new[] { "TenantId", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_AccountId",
                table: "BusinessMembers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_AccountId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "AccountId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountId",
                table: "Users",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_AccountId",
                table: "Users",
                columns: new[] { "TenantId", "AccountId" },
                unique: true,
                filter: "[AccountId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Email",
                table: "Users",
                columns: new[] { "TenantId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_PhoneNumber",
                table: "Users",
                columns: new[] { "TenantId", "PhoneNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMembers_Users_UserId",
                table: "BusinessMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAccounts_Users_UserId",
                table: "ClientAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Users_UserId",
                table: "Instructors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMembers_Users_UserId",
                table: "BusinessMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientAccounts_Users_UserId",
                table: "ClientAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Users_UserId",
                table: "Instructors");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_UserId",
                table: "Instructors");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_UserId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_UserId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_AccountId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ClientAccounts_TenantId_AccountId",
                table: "ClientAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_AccountId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_TenantId_AccountId",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClientAccounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BusinessMembers");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "ClientAccounts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "BusinessMembers",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_UserId",
                table: "ClientAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccounts_TenantId_UserId",
                table: "ClientAccounts",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_UserId",
                table: "BusinessMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_TenantId_UserId",
                table: "BusinessMembers",
                columns: new[] { "TenantId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
