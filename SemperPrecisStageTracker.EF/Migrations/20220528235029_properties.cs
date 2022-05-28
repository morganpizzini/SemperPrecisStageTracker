using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class properties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MedicalExaminationExpireDate",
                table: "Shooters",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthLocation",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirearmsLicenceReleaseDate",
                table: "Shooters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FiscalCode",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Cost",
                table: "Matches",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "PaymentDetails",
                table: "Matches",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "BirthLocation",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "FirearmsLicenceReleaseDate",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "FiscalCode",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "PaymentDetails",
                table: "Matches");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MedicalExaminationExpireDate",
                table: "Shooters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
