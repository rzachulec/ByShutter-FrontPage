using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByShutterBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioTiles_Photos_PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles");

            migrationBuilder.DropIndex(
                name: "IX_PortfolioTiles_PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTiles_ImageId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioTiles_Photos_ImageId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                column: "ImageId",
                principalSchema: "ByShutter",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioTiles_Photos_ImageId",
                schema: "ByShutter",
                table: "PortfolioTiles");

            migrationBuilder.DropIndex(
                name: "IX_PortfolioTiles_ImageId",
                schema: "ByShutter",
                table: "PortfolioTiles");

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTiles_PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioTiles_Photos_PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                column: "PhotoId",
                principalSchema: "ByShutter",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
