using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherSite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Temperature = table.Column<short>(type: "smallint", nullable: false),
                    Humidity = table.Column<byte>(type: "smallint", nullable: false),
                    Td = table.Column<short>(type: "smallint", nullable: false),
                    Pressure = table.Column<int>(type: "integer", nullable: false),
                    DirectionWind = table.Column<string>(type: "text", nullable: false),
                    VelocityWind = table.Column<byte>(type: "smallint", nullable: false),
                    CloudCover = table.Column<byte>(type: "smallint", nullable: true),
                    H = table.Column<int>(type: "integer", nullable: false),
                    VV = table.Column<byte>(type: "smallint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => new { x.Date, x.Time });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
