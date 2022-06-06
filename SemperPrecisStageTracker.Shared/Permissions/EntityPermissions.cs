using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    public static class KnownRoles
    {
        public static string Admin => nameof(Admin);
        public static string TeamHolder => nameof(Admin);
        public static string TeamSecretary => nameof(Admin);
        public static string TeamContributor => nameof(Admin);
        public static string MatchContributor => nameof(Admin);
        public static string MatchSO => nameof(Admin);


        public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof (instance));
            foreach (T obj in instance)
                action(obj);
        }

        public static async Task<IList<TOutput>> AsAsync<TInput, TOutput>(
            this IEnumerable<TInput> instance,
            Func<TInput, Task<TOutput>> convertFunction)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof (instance));
            if (convertFunction == null)
                throw new ArgumentNullException(nameof (convertFunction));
            var outList = new List<TOutput>();
            instance.Each(async s => outList.Add( await convertFunction(s)));
            return outList;
        }
    }
    public enum Permissions
    {
        // admin
        [Description("ManageMatches")]
        ManageMatches = 901,
        [Description("ManageShooters")]
        ManageShooters = 902,
        [Description("ManageTeams")]
        ManageTeams = 903,
        [Description("ManageAssociations")]
        ManageAssociations = 904,
        [Description("ManagePlaces")]
        ManagePlaces = 905,
        [Description("ManageStages")]
        ManageStages = 906,
        [Description("CreateMatches")]
        CreateMatches = 907,
        [Description("CreateShooters")]
        CreateShooters = 908,
        [Description("CreateTeams")]
        CreateTeams = 909,
        [Description("CreateStages")]
        CreateStages = 910,
        [Description("CreateAssociations")]
        CreateAssociations = 911,
        [Description("CreatePlaces")]
        CreatePlaces = 912,
        [Description("ShowShooters")]
        ShowShooters = 913,
        [Description("ManagePermissions")]
        ManagePermissions = 912,

        // entity
        [Description("EditShooter")]
        EditShooter = 1,
        [Description("EditMatch")]
        EditMatch = 14,
        [Description("EditTeam")]
        EditTeam = 5,
        [Description("EditAssociation")]
        EditAssociation = 7,
        [Description("EditPlace")]
        EditPlace = 9,

        [Description("TeamEditShooters")]
        TeamEditShooters = 15,
        [Description("TeamEditPayment")]
        TeamEditPayment = 16,
        [Description("MatchManageGroups")]
        MatchManageGroups = 17,
        [Description("MatchManageStageSO")]
        MatchManageStageSO = 18,
        [Description("MatchInsertScore")]
        MatchInsertScore = 19,
        [Description("MatchManageMD")]
        MatchManageMD = 20,
        [Description("MatchManageStages")]
        MatchManageStages= 21,
        [Description("MatchHandling")]
        MatchHandling = 22
    }

    public interface IPermissionInterface
    {
        IPermissionInterface ManageMatches {get;}
        IPermissionInterface ManageShooters {get;}
        IPermissionInterface ManageTeams {get;}
        IPermissionInterface ManageAssociations {get;}
        IPermissionInterface ManagePlaces {get;}
        IPermissionInterface ManageStages {get;}
        IPermissionInterface CreateMatches {get;}
        IPermissionInterface CreateShooters {get;}
        IPermissionInterface CreateTeams {get;}
        IPermissionInterface CreateStages {get;}
        IPermissionInterface CreateAssociations {get;}
        IPermissionInterface CreatePlaces {get;}
        IPermissionInterface ShowShooters {get;}
        IPermissionInterface ManagePermissions {get;}
        IPermissionInterface EditShooter {get;}
        IPermissionInterface EditMatch {get;}
        IPermissionInterface EditTeam {get;}
        IPermissionInterface EditAssociation {get;}
        IPermissionInterface EditPlace {get;}
        IPermissionInterface TeamEditShooters {get;}
        IPermissionInterface TeamEditPayment {get;}
        IPermissionInterface MatchManageGroups {get;}
        IPermissionInterface MatchManageStageSO {get;}
        IPermissionInterface MatchInsertScore {get;}
        IPermissionInterface MatchManageMD {get;}
        IPermissionInterface MatchManageStages {get;}
        IPermissionInterface MatchHandling {get;}

        IList<Permissions> List { get; }
    }
    public class PermissionHandler : IPermissionInterface
    {
        private readonly IList<Permissions> permissions = new List<Permissions>();
        public IPermissionInterface AddPermission(IList<Permissions> perms)
        {
            foreach (var perm in perms)
            {
                permissions.Add(perm);
            }
            return this;
        }
        public IPermissionInterface AddPermission(Permissions perm)
        {
            permissions.Add(perm);
            return this;
        }
        public IList<Permissions> List => permissions;
        public override string ToString()
        {
            return string.Join(",", permissions.Select(x=>x.ToDescriptionString()));
        }


        public IPermissionInterface ManageMatches => AddPermission(Permissions.ManageMatches);
        public IPermissionInterface ManageShooters => AddPermission(Permissions.ManageShooters);
        public IPermissionInterface ManageTeams => AddPermission(Permissions.ManageTeams);
        public IPermissionInterface ManageAssociations => AddPermission(Permissions.ManageAssociations);
        public IPermissionInterface ManagePlaces => AddPermission(Permissions.ManagePlaces);
        public IPermissionInterface ManageStages => AddPermission(Permissions.ManageStages);
        public IPermissionInterface CreateMatches => AddPermission(Permissions.CreateMatches);
        public IPermissionInterface CreateShooters => AddPermission(Permissions.CreateShooters);
        public IPermissionInterface CreateTeams => AddPermission(Permissions.CreateTeams);
        public IPermissionInterface CreateStages => AddPermission(Permissions.CreateStages);
        public IPermissionInterface CreateAssociations => AddPermission(Permissions.CreateAssociations);
        public IPermissionInterface CreatePlaces => AddPermission(Permissions.CreatePlaces);
        public IPermissionInterface ShowShooters => AddPermission(Permissions.ShowShooters);
        public IPermissionInterface ManagePermissions => AddPermission(Permissions.ManagePermissions);
        public IPermissionInterface EditShooter => AddPermission(Permissions.EditShooter);
        public IPermissionInterface EditMatch => AddPermission(Permissions.EditMatch);
        public IPermissionInterface EditTeam => AddPermission(Permissions.EditTeam);
        public IPermissionInterface EditAssociation => AddPermission(Permissions.EditAssociation);
        public IPermissionInterface EditPlace => AddPermission(Permissions.EditPlace);
        public IPermissionInterface TeamEditShooters => AddPermission(Permissions.TeamEditShooters);
        public IPermissionInterface TeamEditPayment => AddPermission(Permissions.TeamEditPayment);
        public IPermissionInterface MatchManageGroups => AddPermission(Permissions.MatchManageGroups);
        public IPermissionInterface MatchManageStageSO => AddPermission(Permissions.MatchManageStageSO);
        public IPermissionInterface MatchInsertScore => AddPermission(Permissions.MatchInsertScore);
        public IPermissionInterface MatchManageMD => AddPermission(Permissions.MatchManageMD);
        public IPermissionInterface MatchManageStages => AddPermission(Permissions.MatchManageStages);
        public IPermissionInterface MatchHandling => AddPermission(Permissions.MatchHandling);
        public IPermissionInterface Compose(IList<Permissions> perms)
        {
            return AddPermission(perms);
        }
    }

    public static class PermissionCtor
    {
        public static IPermissionInterface Compose(IList<Permissions> perms) => new PermissionHandler().Compose(perms);
        public static IPermissionInterface ManageMatches => new PermissionHandler().ManageMatches;
        public static IPermissionInterface ManageShooters => new PermissionHandler().ManageShooters;
        public static IPermissionInterface ManageTeams => new PermissionHandler().ManageTeams;
        public static IPermissionInterface ManageAssociations => new PermissionHandler().ManageAssociations;
        public static IPermissionInterface ManagePlaces => new PermissionHandler().ManagePlaces;
        public static IPermissionInterface ManageStages => new PermissionHandler().ManageStages;
        public static IPermissionInterface CreateMatches => new PermissionHandler().CreateMatches;
        public static IPermissionInterface CreateShooters => new PermissionHandler().CreateShooters;
        public static IPermissionInterface CreateTeams => new PermissionHandler().CreateTeams;
        public static IPermissionInterface CreateStages => new PermissionHandler().CreateStages;
        public static IPermissionInterface CreateAssociations => new PermissionHandler().CreateAssociations;
        public static IPermissionInterface CreatePlaces => new PermissionHandler().CreatePlaces;
        public static IPermissionInterface ShowShooters => new PermissionHandler().ShowShooters;
        public static IPermissionInterface ManagePermissions => new PermissionHandler().ManagePermissions;
        public static IPermissionInterface EditShooter => new PermissionHandler().EditShooter;
        public static IPermissionInterface EditMatch => new PermissionHandler().EditMatch;
        public static IPermissionInterface EditTeam => new PermissionHandler().EditTeam;
        public static IPermissionInterface EditAssociation => new PermissionHandler().EditAssociation;
        public static IPermissionInterface EditPlace => new PermissionHandler().EditPlace;
        public static IPermissionInterface TeamEditShooters => new PermissionHandler().TeamEditShooters;
        public static IPermissionInterface TeamEditPayment => new PermissionHandler().TeamEditPayment;
        public static IPermissionInterface MatchManageGroups => new PermissionHandler().MatchManageGroups;
        public static IPermissionInterface MatchManageStageSO => new PermissionHandler().MatchManageStageSO;
        public static IPermissionInterface MatchInsertScore => new PermissionHandler().MatchInsertScore;
        public static IPermissionInterface MatchManageMD => new PermissionHandler().MatchManageMD;
        public static IPermissionInterface MatchManageStages => new PermissionHandler().MatchManageStages;
        public static IPermissionInterface MatchHandling => new PermissionHandler().MatchHandling;
    }
}