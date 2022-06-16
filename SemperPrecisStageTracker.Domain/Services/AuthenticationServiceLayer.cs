using SemperPrecisStageTracker.Domain.Clients;
using SemperPrecisStageTracker.Domain.Containers;
using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using log4net.Util;
using Microsoft.Extensions.Caching.Memory;
using SemperPrecisStageTracker.Domain.Cache;
using SemperPrecisStageTracker.Domain.Models;
using SemperPrecisStageTracker.Domain.Utils;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.ServicesLayers;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class AuthenticationServiceLayer : DataServiceLayerBase
    {
        /// <summary>
        /// Designed platform administrator entity name
        /// </summary>
        public const string PlatformAdministratorUserName = "morgan";

        #region Private fields
        private readonly IShooterRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionsRoleRepository _permissionRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IPermissionGroupRepository _permissionGroupRepository;
        private readonly IUserPermissionGroupRepository _userPermissionGroupRepository;
        private readonly IPermissionGroupRoleRepository _permissionGroupRoleRepository;
        private readonly IIdentityClient _identityClient;
        private readonly ISemperPrecisMemoryCache _cache;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSession">Active data session</param>
        public AuthenticationServiceLayer(IDataSession dataSession)
            : base(dataSession)
        {
            //Inizializzazioni
            _userRepository = dataSession.ResolveRepository<IShooterRepository>();
            _permissionRepository = dataSession.ResolveRepository<IPermissionRepository>();            
            _permissionRoleRepository = dataSession.ResolveRepository<IPermissionsRoleRepository>();
            _roleRepository = dataSession.ResolveRepository<IRoleRepository>();
            _userRoleRepository = dataSession.ResolveRepository<IUserRoleRepository>();
            _userPermissionRepository = dataSession.ResolveRepository<IUserPermissionRepository>();
            _permissionGroupRepository = dataSession.ResolveRepository<IPermissionGroupRepository>();
            _userPermissionGroupRepository = dataSession.ResolveRepository<IUserPermissionGroupRepository>();
            _permissionGroupRoleRepository = dataSession.ResolveRepository<IPermissionGroupRoleRepository>();

            _identityClient = ServiceResolver.Resolve<IIdentityClient>();
            _cache = ServiceResolver.Resolve<ISemperPrecisMemoryCache>();
        }

        /// <summary>
        /// Executes sign-in using credentials
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Returns signed in user or null</returns>
        public async Task<Shooter> LogIn(string userName, string password)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            var response = await _identityClient.SignIn(userName, password);

            //Se non ho uno stato valido, esco con null, altrimenti mando in uscita
            //il contenuto della response che dovrebbe già essere uno UserContract
            return response.Response.IsSuccessStatusCode
                ? response.Data
                : null;
        }

        /// <summary>
        /// Update user instagram access token
        /// </summary>
        /// <param name="entity">user entity</param>
        /// <param name="userRoleKey">user role key</param>
        /// <returns>User Entity</returns>
        public IList<ValidationResult> CreateUser(Shooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto è nuovo, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user already exists. Operation aborted");

            // controllo validazione user
            var validations = CheckUserValidation(entity);

            if (validations.Count > 0)
            {
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _userRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            validations = SaveEntity(entity, _userRepository);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();


            return validations;
        }

        /// <summary>
        /// Update user instagram access token
        /// </summary>
        /// <param name="entity">user entity</param>
        /// <param name="userRoleKey">user role key</param>
        /// <returns>User Entity</returns>
        public IList<ValidationResult> UpdateUser(Shooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto è nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Operation aborted");

            // controllo validazione user
            var validations = CheckUserValidation(entity);

            if (validations.Count > 0)
            {
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _userRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            validations = SaveEntity(entity, _userRepository);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();


            return validations;
        }

        /// <summary>
        /// Check user validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckUserValidation(Shooter entity)
        {
            var validations = new List<ValidationResult>();
            // controllo esistenza customer con stesso nome
            var existing = _userRepository.GetSingle(x => x.Id != entity.Id
                                                          && (x.Username == entity.Username
                                                              || x.Email == entity.Email));
            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with username or email already exist. {entity.Username} ({entity.Email})"));
            }

            return validations;
        }

        /// <summary>
        /// Check user password
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newPassword">New password</param>
        /// <returns>Returns signed in user or null</returns>
        public IList<ValidationResult> UpdateUserPassword(Shooter user, string newPassword)
        {
            //Validazione argomenti
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword));

            //aggiorno la password
            user.Password = _identityClient.EncryptPassword(newPassword);
            // reset password alias after set password 
            user.RestorePasswordAlias = string.Empty;

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _userRepository.Validate(user);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _userRepository.Save(user);
                t.Commit();
            }
            return validations;
        }

        

        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Return user permissions list</returns>
        public Task<UserPermissionDto> GetUserPermissionById(string userId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            var cached = _cache.GetValue<UserPermissionDto>($"perm:{userId}");
            if (cached != null)
                return Task.FromResult(cached);
            // user group TODO to be implemented
            // var permissionGroupIds = _userPermissionGroupRepository.FetchWithProjection(x=>x.PermissionGroupId,x => x.UserId == userId);

            
            // user permission
            var userPermissions = _userPermissionRepository.Fetch(x => x.UserId == userId);
            // extract permission
            var permissionIds = userPermissions.Select(x => x.PermissionId).ToList();
            
            // user roles
            var userRoles = _userRoleRepository.Fetch(x => x.UserId == userId);
            var rolesIds = userRoles.Select(x => x.RoleId).ToList();
            var permissionRoles = _permissionRoleRepository.Fetch(x => rolesIds.Contains(x.RoleId));
            permissionIds.AddRange(permissionRoles.Select(x=>x.PermissionId));

            // permission applied
            var permissions = _permissionRepository.Fetch(x => permissionIds.Contains(x.Id));

            var genericPermissionsDto = new List<string>();

            // split permission between entities and general
            // general
            // - roles
            var generalRoles = userRoles.Where(x => string.IsNullOrEmpty(x.EntityId));
            var generalRolesId = generalRoles.Select(x => x.RoleId).ToList();
            var permissionsFromRole = permissionRoles.Where(x => generalRolesId.Contains(x.RoleId))
                .Select(x => x.PermissionId).ToList();

            // - permissions
            var generalPermissionIds = userPermissions.Where(x => string.IsNullOrEmpty(x.EntityId)).Select(x=>x.PermissionId).ToList();
            permissionsFromRole.AddRange(generalPermissionIds);
            
            genericPermissionsDto.AddRange(permissions.Where(x=>permissionsFromRole.Contains(x.Id)).Select(x=>x.Name));
            var genericPermissions = genericPermissionsDto
                                                    .Distinct()
                                                    .Select(x=>x.ParseEnum<Permissions>())
                                                    .ToList();
            
            // entities
            // group by entityId 
            // entity type is not relevant because a the permission name is unique, an entityid associated to 'edit match' will be for sure related to match entity
            // - roles
            var targetingRoles = userRoles.Where(x => !string.IsNullOrEmpty(x.EntityId));
            var entityPermissionList = targetingRoles.GroupBy(x => x.EntityId).Select(x => new
                EntityPermission {
                EntityId = x.Key,
                Permissions = permissionRoles.Where(pr => x.Select(r => r.RoleId).ToList().Contains(pr.RoleId))
                                .Select(pr =>  pr.PermissionId.ParseEnum<Permissions>()).ToList()
            }).ToList();

            // - permissions
            var targetingPermissions = userPermissions.Where(x => !string.IsNullOrEmpty(x.EntityId));
            var tp = targetingPermissions.GroupBy(x => x.EntityId).Select(x => new
                EntityPermissionTmp{
                EntityId = x.Key,
                Permissions  = x.Select(c=>c.PermissionId).ToList()
            }).ToList();

            // merge lists
            foreach (var t in tp)
            {
                var tmp = entityPermissionList.FirstOrDefault(x => x.EntityId == t.EntityId);
                if (tmp == null)
                {
                    entityPermissionList.Add(new EntityPermission()
                    {
                        EntityId = t.EntityId,
                        Permissions = permissions.Where(x => t.Permissions.Contains(x.Id))
                                        .Select(x => x.Name)
                                        .Distinct()
                                        .Select(x=>x.ParseEnum<Permissions>())
                                        .ToList()
                    });
                    continue;
                }
                tmp.Permissions.AddRange(t.Permissions.Select(x=> x.ParseEnum<Permissions>()).ToList());
            }

            var result = new UserPermissionDto
            {
                GenericPermissions = genericPermissions,
                EntityPermissions = entityPermissionList
            };

            _cache.SetValue(userId,result);
            //Recupero i dati, commit ed uscita
            return Task.FromResult(result);
        }

        public class EntityPermissionTmp
        {
            public string EntityId { get; set; } = string.Empty;
            public List<string> Permissions { get; set; } = new ();
        }

        


        public Task<IList<ValidationResult>> DeletePermissionsOnEntity(IList<Permissions> permissions, string entityId)
        {
            var permissionsAsStringList = permissions.Select(x => x.ToDescriptionString()).ToList();
            return this.DeletePermissionsOnEntity(permissionsAsStringList, entityId);
        }

        /// <summary>
        /// Delete all user permissions applied to a single entity
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Return user permissions list</returns>
        public Task<IList<ValidationResult>> DeletePermissionsOnEntity(IList<string> permissions, string entityId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(entityId)) throw new ArgumentNullException(nameof(entityId));
            IList<ValidationResult> validations = new List<ValidationResult>();

            var permissionIds = _permissionRepository.FetchWithProjection(x => x.Id, x => permissions.Contains(x.Name));

            //retrieve elements
            var userPermissions = _userPermissionRepository.Fetch(x => x.EntityId == entityId && permissionIds.Contains(x.PermissionId));
            
            foreach (var entityPermission in userPermissions)
            {
                _userPermissionRepository.Delete(entityPermission);
            }

            // get role permission on entity
            var rolePermissions = _userRoleRepository.Fetch(x => x.EntityId == entityId);

            var roleIds = rolePermissions.Select(x => x.RoleId);

            var rolePermission =
                _permissionRoleRepository
                    .FetchWithProjection(x=>x.RoleId,
                        // filter by permissions
                        x => roleIds.Contains(x.RoleId) && permissionIds.Contains(x.PermissionId));

            // delete every role with entityId and the role which contains the required permission
            foreach (var userRole in rolePermissions.Where(x=>rolePermission.Contains(x.RoleId)))
            {
                _userRoleRepository.Delete(userRole);
            }

            //Recupero i dati, commit ed uscita
            return Task.FromResult(validations);
        }

        /// <summary>
        /// Operation without transaction
        /// </summary>
        /// <param name="newPermissions"></param>
        /// <returns></returns>
        public IList<ValidationResult> SaveUserPermissions(IList<UserPermission> newPermissions)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();
            foreach (var newPermission in newPermissions)
            {
                validations = _userPermissionRepository.Validate(newPermission);
                if (validations.Count > 0)
                    return validations;

                _userPermissionRepository.Save(newPermission);
            }
            return validations;
        }

        /// <summary>
        /// Operation without transaction
        /// </summary>
        /// <param name="newPermissions"></param>
        /// <returns></returns>
        public IList<ValidationResult> SavePermission(Permission entity)
        {
            var validations = _permissionRepository.Validate(entity);
            if (validations.Count > 0)
                return validations;

            _permissionRepository.Save(entity);

            return validations;
        }

        /// <summary>
        /// Operation without transaction
        /// </summary>
        /// <param name="newPermissions"></param>
        /// <returns></returns>
        public IList<ValidationResult> SavePermissionRole(PermissionRole entity)
        {
            
            var validations = _permissionRoleRepository.Validate(entity);
            if (validations.Count > 0)
                return validations;

            _permissionRoleRepository.Save(entity);

            return validations;
        }

        /// <summary>
        /// Operation without transaction
        /// </summary>
        /// <param name="newPermissions"></param>
        /// <returns></returns>
        public IList<ValidationResult> SaveUserRole(UserRole entity)
        {
            
            var validations = _userRoleRepository.Validate(entity);
            if (validations.Count > 0)
                return validations;

            _userRoleRepository.Save(entity);

            return validations;
        }

        /// <summary>
        /// Operation without transaction
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public IList<ValidationResult> SaveRole(Role role)
        {
            var validations = _roleRepository.Validate(role);

            if (validations.Count > 0)
                return validations;

            _roleRepository.Save(role);

            return validations;
        }

        /// <summary>
        /// Get single user by user id
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Return user or null</returns>
        public Shooter GetUserById(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));


            //Recupero i dati, commit ed uscita
            return _userRepository.GetSingle(x => x.Id == id);

        }

        /// <summary>
        /// Get single user by user id
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Return user or null</returns>
        public Shooter GetUserByRestorePasswordAlias(string alias)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));


            //Recupero i dati, commit ed uscita
            return _userRepository.GetSingle(x => x.RestorePasswordAlias == alias);

        }

        /// <summary>
        /// Get single user by user name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Return user or null</returns>
        public Shooter GetUserByUserName(string userName)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));


            return _userRepository.GetSingle(x => x.Username == userName);
        }


        /// <summary>
        /// Checks if provided principal is platform owner
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Returns true if owner</returns>
        public bool IsPlatformOwner(IPrincipal principal)
        {
            //Validazione argomenti
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            //Recupero lo username dal claim
            string username = GetUsernameFromPrincipal(principal);

            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            //Utilizzo il metodo overloaded
            return IsPlatformOwner(username);
        }

        /// <summary>
        /// Checks if provided entity is platform owner
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Returns true if owner</returns>
        public bool IsPlatformOwner(string userName)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));

            return false;
        }

        public IList<Permission> FetchPermissionByNames(IList<string> names)
        {
            //Validazione argomenti
            if (names == null) throw new ArgumentNullException(nameof(names));

            return _permissionRepository.Fetch(x => names.Contains(x.Name));
        }

        public IList<Permission> FetchPermissionsOnRole(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var permissionIds = _permissionRoleRepository.FetchWithProjection(x=>x.PermissionId,x => x.RoleId == id);
            return _permissionRepository.Fetch(x => permissionIds.Contains(x.Id));
        }

        public IList<UserRole> FetchUserRole(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return _userRoleRepository.Fetch(x => x.RoleId == id);
        }

        public IList<ValidationResult> SyncPasswords()
        {
            var shooters = this._userRepository.Fetch(x => string.IsNullOrEmpty(x.Password));
            IList<ValidationResult> validations = new List<ValidationResult>();

            var t = DataSession.BeginTransaction();
            foreach (var shooter in shooters)
            {
                if (string.IsNullOrEmpty(shooter.Username))
                    shooter.Password = shooter.FirstName + shooter.LastName;
                if (string.IsNullOrEmpty(shooter.Email))
                    shooter.Email = $"{shooter.FirstName}{shooter.LastName}@email.com".ToLower();
                shooter.Password = shooter.FirstName + shooter.LastName;

                //Validazione argomenti
                validations = _userRepository.Validate(shooter);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _userRepository.Save(shooter);
            }

            t.Commit();



            return validations;
        }

        
        public PermissionRole GetPermissionRole(string permissionId, string roleId)
        {
            if (string.IsNullOrEmpty(permissionId)) throw new ArgumentNullException(nameof(permissionId));
            if (string.IsNullOrEmpty(roleId)) throw new ArgumentNullException(nameof(roleId));

            return _permissionRoleRepository.GetSingle(x => x.PermissionId == permissionId && x.RoleId == roleId);
        }


        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreatePermissionRole(PermissionRole entity,string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided role permission seems to already existing");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId, PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreatePermissionRole)} with Id: {entity.Id}");
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            
            validations = _permissionRoleRepository.Validate(entity);

            if (validations.Count > 0)
            {
                t.Rollback();
                return validations;
            }
            //Eliminazione
            _permissionRoleRepository.Save(entity);

            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeletePermissionRole(PermissionRole entity,string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided role doesn't have valid Id");
            var validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId, PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeletePermissionRole)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Eliminazione
            _permissionRoleRepository.Delete(entity);

            t.Commit();
            return validations;

        }

        /// <summary>
        /// Fetch user role
        /// </summary>
        /// <returns></returns>
        public IList<Role> FetchRoles() => _roleRepository.Fetch();

        /// <summary>
        /// Fetch permission
        /// </summary>
        /// <returns></returns>
        public IList<Permission> FetchPermission() => _permissionRepository.Fetch();

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public Role GetRole(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _roleRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateRole(Role entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Shooter seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId,PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateRole)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckRoleValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _roleRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _roleRepository.Save(entity);
            
            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateRole(Role entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId, PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateRole)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckRoleValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _roleRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _roleRepository.Save(entity);
            t.Commit();
            return validations;
        }

        /// <summary>
        /// Check shooter validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckRoleValidation(Role entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza shooter con stesso email o licenza
            var existing = _roleRepository.GetSingle(x => x.Id != entity.Id
                                                             &&
                                                             x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name '{entity.Name}' already exists"));
            }

            return validations;
        }
        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteRole(Role entity,string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided role doesn't have valid Id");
            var validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId, PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteRole)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Eliminazione
            _roleRepository.Delete(entity);

            t.Commit();
            return validations;

        }
        
        public UserRole GetUserRole(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return _userRoleRepository.GetSingle(x => x.Id == id);
        }
        public UserRole GetUserRole(string userId, string roleId,string entityId = null)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(roleId)) throw new ArgumentNullException(nameof(roleId));

            return _userRoleRepository.GetSingle(x => x.UserId == userId && x.RoleId==roleId && (string.IsNullOrEmpty(x.EntityId) || x.EntityId == entityId));
        }


        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateUserRole(UserRole entity,string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user role seems to already existing");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId, PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateUserRole)} with Id: {entity.Id}");
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            validations = _userRoleRepository.Validate(entity);

            if (validations.Count > 0)
            {
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _userRoleRepository.Save(entity);

            t.Commit();
            _cache.RemoveValue($"perm:{userId}");
            return validations;

        }

        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteUserRole(UserRole entity,string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user role doesn't have valid Id");
            var validations = new List<ValidationResult>();

            //Check permissions
            if (!await ValidateUserPermissions(userId, PermissionCtor.ManagePermissions))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteUserRole)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Eliminazione
            _userRoleRepository.Delete(entity);

            t.Commit();
            _cache.RemoveValue($"perm:{userId}");
            return validations;

        }

        public Permission GetPermissionByName(Permissions role) => GetPermissionByName(role.ToDescriptionString());

        public Permission GetPermissionByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return _permissionRepository.GetSingle(x => x.Name == name);
        }

        public Permission GetPermission(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            return _permissionRepository.GetSingle(x => x.Id == id);
        }
        public Role GetRoleByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return _roleRepository.GetSingle(x => x.Name == name);
        }
        
        public Task<bool> ValidateUserPermissions(string userId, IPermissionInterface permissions)
        {
            return ValidateUserPermissions(userId, string.Empty, permissions);
        }

        public async Task<bool> ValidateUserPermissions(string userId, string entityId, IPermissionInterface permissions)
        {
            IList<Permissions> shooterPermission = permissions.List;
            if (string.IsNullOrEmpty(userId) || shooterPermission == null || shooterPermission.Count == 0)
            {
                return false;
            }
            var userPermissions = await GetUserPermissionById(userId);
            
            if (userPermissions == null)
                return false;

            // AuthenticationService.CheckPermissions
            return 
                userPermissions.GenericPermissions.Any(shooterPermission.Contains) ||
                    userPermissions.EntityPermissions.Any(x=> (string.IsNullOrEmpty(entityId) || x.EntityId== entityId) && x.Permissions.Any(shooterPermission.Contains));
        }

        #region Private methods

        /// <summary>
        /// get username from principal
        /// </summary>
        /// <param name="principal">principal</param>
        /// <returns>username</returns>
        private string GetUsernameFromPrincipal(IPrincipal principal)
        {
            //Validazione argomenti
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            //Se l'identity non è valida o non sono autenticato, fallisco
            if (principal.Identity == null || !principal.Identity.IsAuthenticated)
                return null;

            return principal.Identity.Name;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">Explicit dispose</param>
        protected new virtual void Dispose(bool isDisposing)
        {
            //Se siamo in rilascio
            if (isDisposing)
            {
                //Rilascio le risorse
                _userRepository.Dispose();
            }

            //Chiamo il metodo base
            base.Dispose(isDisposing);
        }


    }
}