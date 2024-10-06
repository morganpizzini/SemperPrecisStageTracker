using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SemperPrecisStageTracker.Models;
using Microsoft.Extensions.Configuration;
using SemperPrecisStageTracker.Domain.Containers;

namespace SemperPrecisStageTracker.EF.Context
{
    // cd SemperPrecisStageTracker.EF/
    // dotnet ef migrations add InitialCreate --startup-project ../SemperPrecisStageTracker.API
    // dotnet ef database update --startup-project ../SemperPrecisStageTracker.API
    // dotnet ef migrations script <latest-migration-name> --startup-project  ../SemperPrecisStageTracker.API -o aa.txt
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
            var configuration = ServiceResolver.Resolve<IConfiguration>();
            var enableDev = bool.Parse(configuration["enableDev"] ?? "false");
            if (enableDev)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
            optionsBuilder.UseSqlServer(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<string>()
                //.AreUnicode(false)
                //.AreFixedLength()
                .HaveMaxLength(255);

            configurationBuilder.Properties<decimal>()
                .HavePrecision(5, 2);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Group>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<BaySchedule>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<BaySchedule>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Reservation>()
               .HasKey(f => f.Id);
            modelBuilder.Entity<Reservation>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Reservation>().Property(f => f.Demands).HasMaxLength(1000);


            modelBuilder.Entity<Schedule>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Schedule>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<GroupUser>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<GroupUser>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserData>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserData>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserTeam>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserTeam>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<Match>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Match>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Bay>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Bay>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Place>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Place>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PlaceData>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<PlaceData>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Association>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Association>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<StageString>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<StageString>()
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

            modelBuilder.Entity<Association>()
                .Property(e => e.Categories)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            
            modelBuilder.Entity<Association>()
                .Property(e => e.MatchKinds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            
            modelBuilder.Entity<Association>()
                .Property(e => e.SoRoles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<TeamHolder>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<TeamHolder>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<TeamPayment>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<TeamPayment>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<User>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<Stage>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Stage>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<Stage>().Property(f => f.Scenario).HasMaxLength(1000);
            modelBuilder.Entity<Stage>().Property(f => f.GunReadyCondition).HasMaxLength(1000);
            modelBuilder.Entity<Stage>().Property(f => f.StageProcedureNotes).HasMaxLength(1000);
            modelBuilder.Entity<Stage>().Property(f => f.StageProcedure).HasMaxLength(1000);

            modelBuilder.Entity<UserStageString>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserStageString>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserStageString>()
                .Property(e => e.DownPoints)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            var valueComparerString = new ValueComparer<IList<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            var valueComparerInt = new ValueComparer<IList<int>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

             modelBuilder.Entity<Association>()
                .Property(e => e.MatchKinds)
                .Metadata
                .SetValueComparer(valueComparerString);
            modelBuilder.Entity<Association>()
                .Property(e => e.Divisions)
                .Metadata
                .SetValueComparer(valueComparerString);
            modelBuilder.Entity<Association>()
                .Property(e => e.Categories)
                .Metadata
                .SetValueComparer(valueComparerString);
            modelBuilder.Entity<Association>()
                .Property(e => e.SoRoles)
                .Metadata
                .SetValueComparer(valueComparerString);

            modelBuilder.Entity<UserStageString>()
                .Property(e => e.DownPoints)
                .Metadata
                .SetValueComparer(valueComparerInt);

            modelBuilder.Entity<Association>()
                .Property(e => e.Classifications)
                .Metadata
                .SetValueComparer(valueComparerString);

            modelBuilder.Entity<Team>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Team>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserAssociation>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserAssociation>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserAssociationInfo>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserAssociationInfo>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserAssociationInfo>()
                .Property(e => e.Categories)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<UserAssociationInfo>()
                .Property(e => e.Categories)
                .Metadata
                .SetValueComparer(valueComparerString);

            modelBuilder.Entity<NotificationSubscription>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<NotificationSubscription>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRelationRole>().ToTable("ShooterRoles")
                .HasKey(f => f.Id);

            modelBuilder.Entity<UserRelationRole>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Contact>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Contact>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

             modelBuilder.Entity<PaymentType>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<PaymentType>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

             modelBuilder.Entity<TeamReminder>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<TeamReminder>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PermissionRole>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<PermissionRole>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Role>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Role>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRole>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserRole>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserPermission>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserPermission>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PermissionGroup>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<PermissionGroup>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserPermissionGroup>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<UserPermissionGroup>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PermissionGroupRole>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<PermissionGroupRole>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

        }

        /// <summary>
        /// List of groups
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        public DbSet<GroupUser> GroupShooters { get; set; }

        /// <summary>
        /// List of team shooters
        /// </summary>
        public DbSet<UserTeam> ShooterTeams { get; set; }

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
        public DbSet<PlaceData> PlaceDatas { get; set; }

        /// <summary>
        /// List of associations
        /// </summary>
        public DbSet<Association> Associations { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        public DbSet<User> Shooters { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        public DbSet<UserData> ShooterDatas { get; set; }

        /// <summary>
        /// List of stage
        /// </summary>
        public DbSet<Stage> Stages { get; set; }

        /// <summary>
        /// List of stageStrings
        /// </summary>
        public DbSet<StageString> StageStrings { get; set; }

        /// <summary>
        /// List of shooter stage
        /// </summary>
        public DbSet<UserStageString> ShooterStages { get; set; }

        /// <summary>
        /// List of shooter team payment
        /// </summary>
        public DbSet<TeamPayment> ShooterTeamPayments { get; set; }


        /// <summary>
        /// List of teams
        /// </summary>
        public DbSet<Team> Teams { get; set; }

        /// <summary>
        /// List of teams
        /// </summary>
        public DbSet<UserAssociation> ShooterAssociations { get; set; }

        /// <summary>
        /// List of teams
        /// </summary>
        public DbSet<UserAssociationInfo> ShooterAssociationInfos { get; set; }

        /// <summary>
        /// List of notification subscription
        /// </summary>
        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }

        /// <summary>
        /// List of shooter matches directors
        /// </summary>
        public DbSet<UserMatch> ShooterMatches { get; set; }

        /// <summary>
        /// List of shooter PSO stages
        /// </summary>
        public DbSet<UserSOStage> ShooterSOStages { get; set; }

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
        public DbSet<PermissionRole> PermissionsRoles { get; set; }

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

        /// <summary>
        /// List of team reminder
        /// </summary>
        public DbSet<TeamReminder> TeamReminder { get; set; }

        /// <summary>
        /// List of payment type
        /// </summary>
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Bay> Bays { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<BaySchedule> BaySchedules { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

    }
}
