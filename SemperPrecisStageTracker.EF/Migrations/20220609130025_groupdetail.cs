using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class groupdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Groups",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxShooterNumber",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MaxShooterNumber",
                table: "Groups");
        }
    }
}
