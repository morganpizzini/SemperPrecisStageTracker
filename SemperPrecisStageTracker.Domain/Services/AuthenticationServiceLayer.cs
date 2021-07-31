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
using SemperPrecisStageTracker.Domain.Models;
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
        private readonly IEntityPermissionRepository _entityPermissionRepository;
        private readonly IAdministrationPermissionRepository _administrationPermissionRepository;
        private readonly IIdentityClient _identityClient;
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
            _entityPermissionRepository = dataSession.ResolveRepository<IEntityPermissionRepository>();
            _administrationPermissionRepository = dataSession.ResolveRepository<IAdministrationPermissionRepository>();
            _identityClient = ServiceResolver.Resolve<IIdentityClient>();
        }

        /// <summary>
        /// Executes sign-in using credentials
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Returns signed in user or null</returns>
        public async Task<Shooter> SignIn(string userName, string password)
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

        private (IList<string> adminPermissions, IList<EntityPermission> entityPermissions) userPermissions = 
            (new List<string>(),new List<EntityPermission>());
        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Return user permissions list</returns>
        public Task<(IList<string> adminPermissions, IList<EntityPermission> entityPermissions)> GetUserPermissionById(string userId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            if (userPermissions.entityPermissions.Count == 0 && userPermissions.adminPermissions.Count == 0)
            {
                userPermissions = (
                    _administrationPermissionRepository.FetchWithProjection(x => x.Permission, x => x.ShooterId == userId),
                    _entityPermissionRepository.Fetch(x => x.ShooterId == userId)
                );
            }

            //Recupero i dati, commit ed uscita
            return Task.FromResult(userPermissions);
        }

        public Task<IList<ValidationResult>> DeletePermissionsOnEntity(IList<EntityPermissions> permissions, string entityId)
        {
            var permissionsAsStringList = permissions.Select(x => x.ToDescriptionString()).ToList();
            return this.DeletePermissionsOnEntity(permissionsAsStringList, entityId);
        }

        /// <summary>
        /// Delete user permissions
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Return user permissions list</returns>
        public Task<IList<ValidationResult>> DeletePermissionsOnEntity(IList<string> permissions, string entityId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(entityId)) throw new ArgumentNullException(nameof(entityId));
            IList<ValidationResult> validations = new List<ValidationResult>();

            //retrieve elements
            var existing =
                _entityPermissionRepository.Fetch(x => x.EntityId == entityId && permissions.Contains(x.Permission));

            foreach (var entityPermission in existing)
            {
                _entityPermissionRepository.Delete(entityPermission);
            }

            //Recupero i dati, commit ed uscita
            return Task.FromResult(validations);
        }

        /// <summary>
        /// Operation without transaction
        /// </summary>
        /// <param name="newEntityPermissions"></param>
        /// <returns></returns>
        public IList<ValidationResult> SaveUserPermissions(IList<EntityPermission> newEntityPermissions)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();
            foreach (var newEntityPermission in newEntityPermissions)
            {
                validations = _entityPermissionRepository.Validate(newEntityPermission);
                if (validations.Count > 0)
                    return validations;

                _entityPermissionRepository.Save(newEntityPermission);

            }
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

        public Task<bool> ValidateUserPermissions(string userId,IList<AdministrationPermissions> adminPermission)
        {
            return ValidateUserPermissions(userId, string.Empty, adminPermission, new List<EntityPermissions>());
        }
        public async Task<bool> ValidateUserPermissions(string userId, string entityId, IList<AdministrationPermissions> adminPermission, IList<EntityPermissions> shooterPermission)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            var rawUserPermissions = await GetUserPermissionById(userId);

            // Convert user permission in application enum permissions
            var permissions = new UserPermissions()
            {
                AdministratorPermissions =
                    rawUserPermissions.adminPermissions.Select(x => x.ParseEnum<AdministrationPermissions>()).ToList(),
                EntityPermissions = rawUserPermissions.entityPermissions.GroupBy(x => x.EntityId)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(e => e.Permission.ParseEnum<EntityPermissions>()).ToList())
            };

            // check for permissions
            if (permissions.AdministratorPermissions.Any(adminPermission.Contains))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(entityId) && shooterPermission.Any() && permissions.EntityPermissions.ContainsKey(entityId))
            {
                var existingPermission = permissions.EntityPermissions
                    .First(sp => sp.Key == entityId).Value;

                if (existingPermission.Any(shooterPermission.Contains))
                    return true;
            }

            return false;
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