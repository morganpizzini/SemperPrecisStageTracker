using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class CleanUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SO",
                table: "Stages");

            migrationBuilder.AddColumn<string>(
                name: "MatchId",
                table: "GroupShooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "GroupShooters");

            migrationBuilder.AddColumn<string>(
                name: "SO",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
