using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByShutterBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumPhotos_Photos_PhotoId",
                schema: "ByShutter",
                table: "AlbumPhotos");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumPhotos_Photos_PhotoId",
                schema: "ByShutter",
                table: "AlbumPhotos",
                column: "PhotoId",
                principalSchema: "ByShutter",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumPhotos_Photos_PhotoId",
                schema: "ByShutter",
                table: "AlbumPhotos");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumPhotos_Photos_PhotoId",
                schema: "ByShutter",
                table: "AlbumPhotos",
                column: "PhotoId",
                principalSchema: "ByShutter",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
