using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PSO",
                table: "Stages",
                newName: "SO");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Matches",
                newName: "PlaceId");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ShooterStages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirearmsLicence",
                table: "Shooters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirearmsLicenceExpireDate",
                table: "Shooters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicalExaminationExpireDate",
                table: "Shooters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "ShooterAssociations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "ShooterAssociations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "ShooterAssociations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SafetyOfficier",
                table: "ShooterAssociations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Holder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterPermissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterTeamPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotifyExpiration = table.Column<bool>(type: "bit", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterTeamPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamHolders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamHolders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "ShooterPermissions");

            migrationBuilder.DropTable(
                name: "ShooterRoles");

            migrationBuilder.DropTable(
                name: "ShooterTeamPayments");

            migrationBuilder.DropTable(
                name: "TeamHolders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ShooterStages");

            migrationBuilder.DropColumn(
                name: "FirearmsLicence",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "FirearmsLicenceExpireDate",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "MedicalExaminationExpireDate",
                table: "Shooters");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "ShooterAssociations");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "ShooterAssociations");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "ShooterAssociations");

            migrationBuilder.DropColumn(
                name: "SafetyOfficier",
                table: "ShooterAssociations");

            migrationBuilder.RenameColumn(
                name: "SO",
                table: "Stages",
                newName: "PSO");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "Matches",
                newName: "Location");
        }
    }
}
