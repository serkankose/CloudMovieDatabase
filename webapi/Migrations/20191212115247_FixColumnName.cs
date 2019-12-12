using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudMovieDatabase.Migrations
{
    public partial class FixColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Actors");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Actors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Actors");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Actors",
                type: "TEXT",
                nullable: true);
        }
    }
}
