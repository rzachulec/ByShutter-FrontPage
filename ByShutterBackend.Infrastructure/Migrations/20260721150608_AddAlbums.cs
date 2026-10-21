using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByShutterBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlbums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Albums_Name",
                schema: "ByShutter",
                table: "Albums",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Albums_Name",
                schema: "ByShutter",
                table: "Albums");
        }
    }
}
