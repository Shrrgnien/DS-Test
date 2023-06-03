using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DS_Test.Migrations
{
    /// <inheritdoc />
    public partial class FormatFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "WeatherRecords");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "WeatherRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "WeatherRecords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "WeatherRecords",
                type: "datetime2",
                nullable: true);
        }
    }
}
