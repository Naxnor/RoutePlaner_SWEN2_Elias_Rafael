using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RoutePlaner_Rafael_elias.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tour",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    From = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    To = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RouteType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartLatitude = table.Column<double>(type: "double precision", nullable: false),
                    StartLongitude = table.Column<double>(type: "double precision", nullable: false),
                    EndLatitude = table.Column<double>(type: "double precision", nullable: false),
                    EndLongitude = table.Column<double>(type: "double precision", nullable: false),
                    EncodedRoute = table.Column<string>(type: "text", nullable: true),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tour", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Distance = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration = table.Column<decimal>(type: "numeric", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Steps = table.Column<decimal>(type: "numeric", nullable: false),
                    Weather = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTime = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourLog_Tour_TourId",
                        column: x => x.TourId,
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tour",
                columns: new[] { "Id", "Description", "Distance", "EncodedRoute", "EndLatitude", "EndLongitude", "EstimatedTime", "From", "Name", "RouteType", "StartLatitude", "StartLongitude", "To" },
                values: new object[] { 1, "Dies ist eine Test-Tour.", 80.0, "", 48.224672649565186, 16.34765625, new TimeSpan(0, 1, 30, 0, 0), "Mank", "TestTour", "driving-car", 48.136766679692691, 15.64453125, "Wien" });

            migrationBuilder.CreateIndex(
                name: "IX_TourLog_TourId",
                table: "TourLog",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourLog");

            migrationBuilder.DropTable(
                name: "Tour");
        }
    }
}
