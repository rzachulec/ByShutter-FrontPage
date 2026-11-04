using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByShutterBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlbumDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                schema: "ByShutter",
                table: "Albums",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                schema: "ByShutter",
                table: "Albums");
        }
    }
}
