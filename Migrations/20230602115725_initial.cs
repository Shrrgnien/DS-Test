using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DS_Test.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Temperature = table.Column<double>(type: "float", nullable: true),
                    AirHumidity = table.Column<double>(type: "float", nullable: true),
                    Td = table.Column<double>(type: "float", nullable: true),
                    Pressure = table.Column<double>(type: "float", nullable: true),
                    AirFlowDirection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirFlowSpeed = table.Column<double>(type: "float", nullable: true),
                    Cloudiness = table.Column<double>(type: "float", nullable: true),
                    h = table.Column<double>(type: "float", nullable: true),
                    VV = table.Column<double>(type: "float", nullable: true),
                    WeatherEvents = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherRecords");
        }
    }
}
