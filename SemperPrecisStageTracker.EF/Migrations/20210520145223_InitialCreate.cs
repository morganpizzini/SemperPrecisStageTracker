using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SemperPrecisStageTracker.EF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Associations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Divisions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ranks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Associations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupShooters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DivisionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupShooters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssociationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterAssociations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AssociationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterAssociations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shooters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shooters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterStages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPoints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Procedurals = table.Column<int>(type: "int", nullable: false),
                    HitOnNonThreat = table.Column<int>(type: "int", nullable: false),
                    FlagrantPenalties = table.Column<int>(type: "int", nullable: false),
                    Ftdr = table.Column<int>(type: "int", nullable: false),
                    Procedural = table.Column<bool>(type: "bit", nullable: false),
                    Disqualified = table.Column<bool>(type: "bit", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShooterTeams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShooterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShooterTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MatchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Targets = table.Column<int>(type: "int", nullable: false),
                    SO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scenario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GunReadyCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StageProcedure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StageProcedureNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scoring = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetsDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScoredHits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartStop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Distance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverGarment = table.Column<bool>(type: "bit", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Associations");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "GroupShooters");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "ShooterAssociations");

            migrationBuilder.DropTable(
                name: "Shooters");

            migrationBuilder.DropTable(
                name: "ShooterStages");

            migrationBuilder.DropTable(
                name: "ShooterTeams");

            migrationBuilder.DropTable(
                name: "Stages");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
