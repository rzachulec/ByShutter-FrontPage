using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByShutterBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PhotoUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ByShutter");

            migrationBuilder.CreateTable(
                name: "Albums",
                schema: "ByShutter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                schema: "ByShutter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ServerPath = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: false),
                    ImageAlt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlbumPhotos",
                schema: "ByShutter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedToAlbumAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumPhotos_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalSchema: "ByShutter",
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumPhotos_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalSchema: "ByShutter",
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeProfiles",
                schema: "ByShutter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroMessage = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Welcome to my page!"),
                    HeroImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Greetings = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Hello!"),
                    Quote = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, defaultValue: "This is a quote!"),
                    AboutMe = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: "This is a short description about me!"),
                    ProfileImageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeProfiles_Photos_HeroImageId",
                        column: x => x.HeroImageId,
                        principalSchema: "ByShutter",
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeProfiles_Photos_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalSchema: "ByShutter",
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioTiles",
                schema: "ByShutter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeProfileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioTiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioTiles_HomeProfiles_HomeProfileId",
                        column: x => x.HomeProfileId,
                        principalSchema: "ByShutter",
                        principalTable: "HomeProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PortfolioTiles_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalSchema: "ByShutter",
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumPhotos_AlbumId",
                schema: "ByShutter",
                table: "AlbumPhotos",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumPhotos_PhotoId",
                schema: "ByShutter",
                table: "AlbumPhotos",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeProfiles_HeroImageId",
                schema: "ByShutter",
                table: "HomeProfiles",
                column: "HeroImageId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeProfiles_ProfileImageId",
                schema: "ByShutter",
                table: "HomeProfiles",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_FileName",
                schema: "ByShutter",
                table: "Photos",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTiles_HomeProfileId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                column: "HomeProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTiles_PhotoId",
                schema: "ByShutter",
                table: "PortfolioTiles",
                column: "PhotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumPhotos",
                schema: "ByShutter");

            migrationBuilder.DropTable(
                name: "PortfolioTiles",
                schema: "ByShutter");

            migrationBuilder.DropTable(
                name: "Albums",
                schema: "ByShutter");

            migrationBuilder.DropTable(
                name: "HomeProfiles",
                schema: "ByShutter");

            migrationBuilder.DropTable(
                name: "Photos",
                schema: "ByShutter");
        }
    }
}
