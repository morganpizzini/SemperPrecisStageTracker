using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class shooterstagespoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bonus",
                table: "ShooterStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "FirstProceduralPointDown",
                table: "ShooterStages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "HitOnNonThreatPointDown",
                table: "ShooterStages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SecondProceduralPointDown",
                table: "ShooterStages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ThirdProceduralPointDown",
                table: "ShooterStages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "ShooterRoles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FirstPenaltyLabel",
                table: "Associations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "FirstProceduralPointDown",
                table: "Associations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "HitOnNonThreatDownPoints",
                table: "Associations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "SecondPenaltyLabel",
                table: "Associations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "SecondProceduralPointDown",
                table: "Associations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "SoRoles",
                table: "Associations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThirdPenaltyLabel",
                table: "Associations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "ThirdProceduralPointDown",
                table: "Associations",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bonus",
                table: "ShooterStages");

            migrationBuilder.DropColumn(
                name: "FirstProceduralPointDown",
                table: "ShooterStages");

            migrationBuilder.DropColumn(
                name: "HitOnNonThreatPointDown",
                table: "ShooterStages");

            migrationBuilder.DropColumn(
                name: "SecondProceduralPointDown",
                table: "ShooterStages");

            migrationBuilder.DropColumn(
                name: "ThirdProceduralPointDown",
                table: "ShooterStages");

            migrationBuilder.DropColumn(
                name: "FirstPenaltyLabel",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "FirstProceduralPointDown",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "HitOnNonThreatDownPoints",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "SecondPenaltyLabel",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "SecondProceduralPointDown",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "SoRoles",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "ThirdPenaltyLabel",
                table: "Associations");

            migrationBuilder.DropColumn(
                name: "ThirdProceduralPointDown",
                table: "Associations");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "ShooterRoles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
