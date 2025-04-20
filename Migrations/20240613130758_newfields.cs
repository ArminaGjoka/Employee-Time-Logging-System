using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timbratura_Testo.Migrations
{
    /// <inheritdoc />
    public partial class newfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RoundedEntryTime",
                table: "Timbrature",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RoundedExitTime",
                table: "Timbrature",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoundedEntryTime",
                table: "Timbrature");

            migrationBuilder.DropColumn(
                name: "RoundedExitTime",
                table: "Timbrature");
        }
    }
}
