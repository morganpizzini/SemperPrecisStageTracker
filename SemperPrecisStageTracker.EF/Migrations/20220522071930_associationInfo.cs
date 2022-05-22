using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class associationInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "ShooterAssociations");

            migrationBuilder.DropColumn(
                name: "SafetyOfficier",
                table: "ShooterAssociations");

            migrationBuilder.RenameColumn(
                name: "MatchDateTime",
                table: "Matches",
                newName: "MatchDateTimeStart");

            migrationBuilder.AlterColumn<string>(
                name: "StageProcedureNotes",
                table: "Stages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StageProcedure",
                table: "Stages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Scenario",
                table: "Stages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MatchDateTimeEnd",
                table: "Matches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "Associations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ShooterAssociationInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AssociationId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Categories = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafetyOfficier = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterAssociationInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShooterAssociationInfos");

            migrationBuilder.DropColumn(
                name: "MatchDateTimeEnd",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Associations");

            migrationBuilder.RenameColumn(
                name: "MatchDateTimeStart",
                table: "Matches",
                newName: "MatchDateTime");

            migrationBuilder.AlterColumn<string>(
                name: "StageProcedureNotes",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StageProcedure",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Scenario",
                table: "Stages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "ShooterAssociations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SafetyOfficier",
                table: "ShooterAssociations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
