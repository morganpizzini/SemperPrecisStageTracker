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
        public static string TeamHolder => nameof(TeamHolder);
        public static string Contributor => nameof(Contributor);
        public static string TeamSecretary => nameof(TeamSecretary);
        public static string TeamContributor => nameof(TeamContributor);
        public static string MatchContributor => nameof(MatchContributor);
        public static string MatchSO => nameof(MatchSO);


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
        [Description("ManageUsers")]
        ManageUsers = 902,
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
        [Description("CreateUser")]
        CreateUser = 908,
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
        ManagePermissions = 914,

        // entity
        [Description("EditUser")]
        EditUser = 1,
        [Description("ShooterDelete")]
        UserDelete = 2,
        [Description("EditTeam")]
        EditTeam = 5,
        [Description("AssociationDelete")]
        AssociationDelete = 6,
        [Description("AssociationEdit")]
        AssociationEdit = 7,
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
        MatchHandling = 22,
        [Description("MatchDelete")]
        MatchDelete = 23,
        [Description("PlaceDelete")]
        PlaceDelete = 24,
        [Description("TeamDelete")]
        TeamDelete = 25
    }

    public interface IPermissionInterface
    {
        IPermissionInterface ManageMatches {get;}
        IPermissionInterface ManageUsers {get;}
        IPermissionInterface ManageTeams {get;}
        IPermissionInterface ManageAssociations {get;}
        IPermissionInterface ManagePlaces {get;}
        IPermissionInterface ManageStages {get;}
        IPermissionInterface CreateMatches {get;}
        IPermissionInterface CreateUser {get;}
        IPermissionInterface CreateTeams {get;}
        IPermissionInterface CreateStages {get;}
        IPermissionInterface CreateAssociations {get;}
        IPermissionInterface CreatePlaces {get;}
        IPermissionInterface ShowShooters {get;}
        IPermissionInterface ManagePermissions {get;}
        IPermissionInterface EditUser {get;}
        IPermissionInterface ShooterDelete {get;}
        IPermissionInterface EditTeam {get;}
        IPermissionInterface EditPlace {get;}
        IPermissionInterface TeamEditShooters {get;}
        IPermissionInterface TeamEditPayment {get;}
        IPermissionInterface MatchManageGroups {get;}
        IPermissionInterface MatchManageStageSO {get;}
        IPermissionInterface MatchInsertScore {get;}
        IPermissionInterface MatchManageMD {get;}
        IPermissionInterface MatchManageStages {get;}
        IPermissionInterface MatchHandling {get;}
        IPermissionInterface MatchDelete {get;}
        IPermissionInterface AssociationEdit {get;}
        IPermissionInterface AssociationDelete {get;}
        

        IList<Permissions> List { get; }
        int Count { get; }
        bool Find(Permissions permission);
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
        public int Count => permissions.Count;
        public bool Find(Permissions permission) => permissions.Contains(permission);
        public override string ToString()
        {
            return string.Join(",", permissions.Select(x=>x.ToDescriptionString()));
        }


        public IPermissionInterface ManageMatches => AddPermission(Permissions.ManageMatches);
        public IPermissionInterface ManageUsers => AddPermission(Permissions.ManageUsers);
        public IPermissionInterface ManageTeams => AddPermission(Permissions.ManageTeams);
        public IPermissionInterface ManageAssociations => AddPermission(Permissions.ManageAssociations);
        public IPermissionInterface ManagePlaces => AddPermission(Permissions.ManagePlaces);
        public IPermissionInterface ManageStages => AddPermission(Permissions.ManageStages);
        public IPermissionInterface CreateMatches => AddPermission(Permissions.CreateMatches);
        public IPermissionInterface CreateUser => AddPermission(Permissions.CreateUser);
        public IPermissionInterface CreateTeams => AddPermission(Permissions.CreateTeams);
        public IPermissionInterface CreateStages => AddPermission(Permissions.CreateStages);
        public IPermissionInterface CreateAssociations => AddPermission(Permissions.CreateAssociations);
        public IPermissionInterface CreatePlaces => AddPermission(Permissions.CreatePlaces);
        public IPermissionInterface ShowShooters => AddPermission(Permissions.ShowShooters);
        public IPermissionInterface ManagePermissions => AddPermission(Permissions.ManagePermissions);
        public IPermissionInterface EditUser => AddPermission(Permissions.EditUser);
        public IPermissionInterface ShooterDelete => AddPermission(Permissions.UserDelete);
        public IPermissionInterface EditTeam => AddPermission(Permissions.EditTeam);
        public IPermissionInterface AssociationEdit => AddPermission(Permissions.AssociationEdit);
        public IPermissionInterface EditPlace => AddPermission(Permissions.EditPlace);
        public IPermissionInterface TeamEditShooters => AddPermission(Permissions.TeamEditShooters);
        public IPermissionInterface TeamEditPayment => AddPermission(Permissions.TeamEditPayment);
        public IPermissionInterface MatchManageGroups => AddPermission(Permissions.MatchManageGroups);
        public IPermissionInterface MatchManageStageSO => AddPermission(Permissions.MatchManageStageSO);
        public IPermissionInterface MatchInsertScore => AddPermission(Permissions.MatchInsertScore);
        public IPermissionInterface MatchManageMD => AddPermission(Permissions.MatchManageMD);
        public IPermissionInterface MatchManageStages => AddPermission(Permissions.MatchManageStages);
        public IPermissionInterface MatchHandling => AddPermission(Permissions.MatchHandling);
        public IPermissionInterface MatchDelete => AddPermission(Permissions.MatchDelete);
        public IPermissionInterface AssociationDelete => AddPermission(Permissions.AssociationDelete);
        
        public IPermissionInterface Compose(IList<Permissions> perms)
        {
            return AddPermission(perms);
        }
    }

    public static class PermissionCtor
    {
        public static IPermissionInterface Compose(IList<Permissions> perms) => new PermissionHandler().Compose(perms);
        public static IPermissionInterface ManageMatches => new PermissionHandler().ManageMatches;
        public static IPermissionInterface ManageUsers => new PermissionHandler().ManageUsers;
        public static IPermissionInterface ManageTeams => new PermissionHandler().ManageTeams;
        public static IPermissionInterface ManageAssociations => new PermissionHandler().ManageAssociations;
        public static IPermissionInterface ManagePlaces => new PermissionHandler().ManagePlaces;
        public static IPermissionInterface ManageStages => new PermissionHandler().ManageStages;
        public static IPermissionInterface CreateMatches => new PermissionHandler().CreateMatches;
        public static IPermissionInterface CreateUser => new PermissionHandler().CreateUser;
        public static IPermissionInterface CreateTeams => new PermissionHandler().CreateTeams;
        public static IPermissionInterface CreateStages => new PermissionHandler().CreateStages;
        public static IPermissionInterface CreateAssociations => new PermissionHandler().CreateAssociations;
        public static IPermissionInterface CreatePlaces => new PermissionHandler().CreatePlaces;
        public static IPermissionInterface ShowShooters => new PermissionHandler().ShowShooters;
        public static IPermissionInterface ManagePermissions => new PermissionHandler().ManagePermissions;
        public static IPermissionInterface EditUser => new PermissionHandler().EditUser;
        public static IPermissionInterface ShooterDelete => new PermissionHandler().ShooterDelete;
        public static IPermissionInterface MatchDelete => new PermissionHandler().MatchDelete;
        public static IPermissionInterface EditTeam => new PermissionHandler().EditTeam;
        public static IPermissionInterface EditPlace => new PermissionHandler().EditPlace;
        public static IPermissionInterface TeamEditShooters => new PermissionHandler().TeamEditShooters;
        public static IPermissionInterface TeamEditPayment => new PermissionHandler().TeamEditPayment;
        public static IPermissionInterface MatchManageGroups => new PermissionHandler().MatchManageGroups;
        public static IPermissionInterface MatchManageStageSO => new PermissionHandler().MatchManageStageSO;
        public static IPermissionInterface MatchInsertScore => new PermissionHandler().MatchInsertScore;
        public static IPermissionInterface MatchManageMD => new PermissionHandler().MatchManageMD;
        public static IPermissionInterface MatchManageStages => new PermissionHandler().MatchManageStages;
        public static IPermissionInterface MatchHandling => new PermissionHandler().MatchHandling;
        public static IPermissionInterface AssociationEdit => new PermissionHandler().AssociationEdit;
        public static IPermissionInterface AssociationDelete => new PermissionHandler().AssociationDelete;
    }
}