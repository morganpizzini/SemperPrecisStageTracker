using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class sample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HitOnNonThreatDownPoints",
                table: "Associations",
                newName: "HitOnNonThreatPointDown");

            migrationBuilder.AddColumn<DateTime>(
                name: "HasPay",
                table: "GroupShooters",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPay",
                table: "GroupShooters");

            migrationBuilder.RenameColumn(
                name: "HitOnNonThreatPointDown",
                table: "Associations",
                newName: "HitOnNonThreatDownPoints");
        }
    }
}
