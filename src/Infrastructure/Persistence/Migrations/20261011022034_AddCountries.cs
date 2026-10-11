using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Businesses",
                type: "nchar(2)",
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CallingCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    OwnAccountMinimumAge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CountryTimeZones",
                columns: table => new
                {
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CountryCode = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryTimeZones", x => new { x.CountryCode, x.TimeZoneId });
                    table.ForeignKey(
                        name: "FK_CountryTimeZones_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Code", "CallingCode", "CurrencyCode", "DisplayOrder", "OwnAccountMinimumAge" },
                values: new object[,]
                {
                    { "AR", "54", "ARS", 1, 13 },
                    { "BO", "591", "BOB", 5, 16 },
                    { "CL", "56", "CLP", 3, 16 },
                    { "CO", "57", "COP", 8, 16 },
                    { "CR", "506", "CRC", 11, 16 },
                    { "EC", "593", "USD", 7, 16 },
                    { "ES", "34", "EUR", 14, 14 },
                    { "GT", "502", "GTQ", 12, 16 },
                    { "MX", "52", "MXN", 13, 16 },
                    { "PA", "507", "USD", 10, 16 },
                    { "PE", "51", "PEN", 6, 16 },
                    { "PY", "595", "PYG", 4, 16 },
                    { "UY", "598", "UYU", 2, 16 },
                    { "VE", "58", "VES", 9, 16 }
                });

            migrationBuilder.InsertData(
                table: "CountryTimeZones",
                columns: new[] { "CountryCode", "TimeZoneId", "DisplayOrder" },
                values: new object[,]
                {
                    { "AR", "America/Argentina/Buenos_Aires", 1 },
                    { "BO", "America/La_Paz", 1 },
                    { "CL", "America/Santiago", 1 },
                    { "CL", "Pacific/Easter", 2 },
                    { "CO", "America/Bogota", 1 },
                    { "CR", "America/Costa_Rica", 1 },
                    { "EC", "America/Guayaquil", 1 },
                    { "EC", "Pacific/Galapagos", 2 },
                    { "ES", "Atlantic/Canary", 2 },
                    { "ES", "Europe/Madrid", 1 },
                    { "GT", "America/Guatemala", 1 },
                    { "MX", "America/Cancun", 2 },
                    { "MX", "America/Hermosillo", 4 },
                    { "MX", "America/Mazatlan", 3 },
                    { "MX", "America/Mexico_City", 1 },
                    { "MX", "America/Tijuana", 5 },
                    { "PA", "America/Panama", 1 },
                    { "PE", "America/Lima", 1 },
                    { "PY", "America/Asuncion", 1 },
                    { "UY", "America/Montevideo", 1 },
                    { "VE", "America/Caracas", 1 }
                });

            migrationBuilder.Sql(
                """
                UPDATE [Businesses]
                SET [CountryCode] = COALESCE(
                    (SELECT TOP 1 [CountryCode] FROM [CountryTimeZones] WHERE [TimeZoneId] = [Businesses].[TimeZoneId]),
                    (SELECT TOP 1 [Code] FROM [Countries] WHERE [CallingCode] = [Businesses].[DefaultCountryCallingCode]),
                    'AR');

                UPDATE [Businesses]
                SET [DefaultCountryCallingCode] = [Countries].[CallingCode]
                FROM [Businesses]
                INNER JOIN [Countries] ON [Countries].[Code] = [Businesses].[CountryCode];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_CountryCode",
                table: "Businesses",
                column: "CountryCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Countries_CountryCode",
                table: "Businesses",
                column: "CountryCode",
                principalTable: "Countries",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Countries_CountryCode",
                table: "Businesses");

            migrationBuilder.DropTable(
                name: "CountryTimeZones");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_CountryCode",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Businesses");
        }
    }
}
