using Microsoft.EntityFrameworkCore.Migrations;

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class Warning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Procedural",
                table: "ShooterStages",
                newName: "Warning");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Warning",
                table: "ShooterStages",
                newName: "Procedural");
        }
    }
}
