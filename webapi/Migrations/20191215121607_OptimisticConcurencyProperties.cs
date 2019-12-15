using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudMovieDatabase.Migrations
{
    public partial class OptimisticConcurencyProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Movies",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Actors",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "ActorMovie",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ActorMovie");
        }
    }
}
