using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemperPrecisStageTracker.Models;
using Microsoft.Extensions.Configuration;
using SemperPrecisStageTracker.Domain.Containers;

namespace SemperPrecisStageTracker.EF.Context
{
    // cd SemperPrecisStageTracker.EF/
    // dotnet ef migrations add InitialCreate --startup-project ../SemperPrecisStageTracker.API
    // dotnet ef database update --startup-project ../SemperPrecisStageTracker.API
    /// <summary>
    /// Context for SemperPrecisStageTracker
    /// </summary>
    public class SemperPrecisStageTrackerContext : DbContext
    {
        /// <summary>
        /// Connection string for database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Costructor
        /// </summary>
        public SemperPrecisStageTrackerContext()
        {
            var configuration = ServiceResolver.Resolve<IConfiguration>();
            const string SqlDbConnectionStringName = "SqlDb";
            var sqlSetting = configuration.GetConnectionString(SqlDbConnectionStringName);
            if (sqlSetting == null)
                throw new InvalidProgramException("Connection string for database with " +
                                                  $"name '{SqlDbConnectionStringName}' cannot be found");
            ConnectionString = sqlSetting;
            //ConnectionString = sqlSetting.ConnectionString;
        }

        /// <summary>
        /// set configuration properties
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Group>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<GroupShooter>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<GroupShooter>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShooterTeam>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<ShooterTeam>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<Match>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Match>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Place>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Place>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Association>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Association>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<Association>()
                .Property(e => e.Divisions)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<Association>()
                .Property(e => e.Classifications)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<TeamHolder>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<TeamHolder>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShooterTeamPayment>()
    .HasKey(f => f.Id);
            modelBuilder.Entity<ShooterTeamPayment>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();


            modelBuilder.Entity<Shooter>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Shooter>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<Stage>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Stage>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShooterStage>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<ShooterStage>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShooterStage>()
                .Property(e => e.DownPoints)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList());

            modelBuilder.Entity<Team>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Team>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShooterAssociation>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<ShooterAssociation>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<NotificationSubscription>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<NotificationSubscription>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShooterRole>().ToTable("ShooterRoles")
                .HasKey(f => f.Id);

            modelBuilder.Entity<ShooterRole>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Contact>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Contact>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<Permission>().ToTable("ShooterPermissions")
                .HasKey(f => f.Id);

            modelBuilder.Entity<Permission>().Property(f => f.Id)
                .ValueGeneratedOnAdd();

        }

        /// <summary>
        /// List of groups
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        public DbSet<GroupShooter> GroupShooters { get; set; }

        /// <summary>
        /// List of team shooters
        /// </summary>
        public DbSet<ShooterTeam> ShooterTeams { get; set; }

        /// <summary>
        /// List of match
        /// </summary>
        public DbSet<Match> Matches { get; set; }

        /// <summary>
        /// List of associations
        /// </summary>
        public DbSet<Place> Places { get; set; }

        /// <summary>
        /// List of associations
        /// </summary>
        public DbSet<Association> Associations { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        public DbSet<Shooter> Shooters { get; set; }

        /// <summary>
        /// List of stage
        /// </summary>
        public DbSet<Stage> Stages { get; set; }

        /// <summary>
        /// List of shooter stage
        /// </summary>
        public DbSet<ShooterStage> ShooterStages { get; set; }

        /// <summary>
        /// List of shooter team payment
        /// </summary>
        public DbSet<ShooterTeamPayment> ShooterTeamPayments { get; set; }


        /// <summary>
        /// List of teams
        /// </summary>
        public DbSet<Team> Teams { get; set; }

        /// <summary>
        /// List of teams
        /// </summary>
        public DbSet<ShooterAssociation> ShooterAssociations { get; set; }

        /// <summary>
        /// List of notification subscription
        /// </summary>
        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }

        /// <summary>
        /// List of shooter matches
        /// </summary>
        public DbSet<ShooterMatch> ShooterMatches { get; set; }

        /// <summary>
        /// List of shooter PSO stages
        /// </summary>
        public DbSet<ShooterSOStage> ShooterSOStages { get; set; }

        /// <summary>
        /// List of team holders
        /// </summary>
        public DbSet<TeamHolder> TeamHolders { get; set; }

        /// <summary>
        /// List of contact
        /// </summary>
        public DbSet<Contact> Contacts { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }
        
        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<PermissionsRole> PermissionsRoles { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<UserPermission> UserPermissions { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<PermissionGroup> PermissionGroups { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<UserPermissionGroup> UserPermissionGroups { get; set; }

        /// <summary>
        /// List of entity permission
        /// </summary>
        public DbSet<PermissionGroupRole> PermissionGroupRoles { get; set; }

    }
}
