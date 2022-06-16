using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class matchKind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RestorePasswordAlias",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kind",
                table: "Matches",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchKinds",
                table: "Associations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestorePasswordAlias",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchKinds",
                table: "Associations");
        }
    }
}
