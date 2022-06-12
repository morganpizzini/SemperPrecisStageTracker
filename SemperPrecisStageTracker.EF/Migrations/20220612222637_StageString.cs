using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class StageString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverGarment",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "ScoredHits",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "Scoring",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "StartStop",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "Strings",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "Targets",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "TargetsDescription",
                table: "Stages");

            migrationBuilder.AlterColumn<string>(
                name: "GunReadyCondition",
                table: "Stages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StageStringId",
                table: "ShooterStages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StageStrings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Targets = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Scoring = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TargetsDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ScoredHits = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartStop = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Distance = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CoverGarment = table.Column<bool>(type: "bit", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageStrings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StageStrings");

            migrationBuilder.DropColumn(
                name: "StageStringId",
                table: "ShooterStages");

            migrationBuilder.AlterColumn<string>(
                name: "GunReadyCondition",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CoverGarment",
                table: "Stages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Distance",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScoredHits",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scoring",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartStop",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Strings",
                table: "Stages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Targets",
                table: "Stages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TargetsDescription",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
