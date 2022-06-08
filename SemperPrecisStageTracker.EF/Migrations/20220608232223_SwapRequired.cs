using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class SwapRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FirearmsLicence",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "FirearmsLicenceExpireDate",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "FirearmsLicenceReleaseDate",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "FiscalCode",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "MedicalExaminationExpireDate",
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
                name: "Address",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Holder",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "PostalZipCode",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Places");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "GroupShooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Classification",
                table: "GroupShooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateTable(
                name: "PlaceDatas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Holder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostalZipCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterDatas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FirearmsLicence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FirearmsLicenceExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirearmsLicenceReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicalExaminationExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthLocation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FiscalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceDatas");

            migrationBuilder.DropTable(
                name: "ShooterDatas");

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

            migrationBuilder.AddColumn<string>(
                name: "FirearmsLicence",
                table: "Shooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirearmsLicenceExpireDate",
                table: "Shooters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicalExaminationExpireDate",
                table: "Shooters",
                type: "datetime2",
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

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Holder",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalZipCode",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Places",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "GroupShooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Classification",
                table: "GroupShooters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
