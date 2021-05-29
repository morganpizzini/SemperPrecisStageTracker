using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.ServicesLayers;
using SemperPrecisStageTracker.Domain.Utils;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class MainServiceLayer : DataServiceLayerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IAssociationRepository _associationRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IShooterRepository _shooterRepository;
        private readonly IShooterStageRepository _shooterStageRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IGroupShooterRepository _groupShooterRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IShooterTeamRepository _shooterTeamRepository;
        private readonly IShooterAssociationRepository _shooterAssociationRepository;
    private readonly INotificationSubscriptionRepository _notificationsubscriptionRepository;
    public MainServiceLayer(IDataSession dataSession)
            : base(dataSession)
        {
            _teamRepository = dataSession.ResolveRepository<ITeamRepository>();
            _groupRepository = dataSession.ResolveRepository<IGroupRepository>();
            _groupShooterRepository = dataSession.ResolveRepository<IGroupShooterRepository>();
            _matchRepository = dataSession.ResolveRepository<IMatchRepository>();
            _associationRepository = dataSession.ResolveRepository<IAssociationRepository>();
            _shooterRepository = dataSession.ResolveRepository<IShooterRepository>();
            _shooterStageRepository = dataSession.ResolveRepository<IShooterStageRepository>();
            _stageRepository = dataSession.ResolveRepository<IStageRepository>();
            _shooterTeamRepository = dataSession.ResolveRepository<IShooterTeamRepository>();
            _shooterAssociationRepository = dataSession.ResolveRepository<IShooterAssociationRepository>();
            _notificationsubscriptionRepository = dataSession.ResolveRepository<INotificationSubscriptionRepository>();
        }
        #region Match

        /// <summary>
        /// Count list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of matchs</returns>
        public int CountMatchs()
        {
            //Utilizzo il metodo base
            return _matchRepository.Count();
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of matchs</returns>
        public IList<Match> FetchAllMatchs()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.MatchDateTime, true, _matchRepository);
        }

        /// <summary>
        /// Fetch list of matchs by provided ids
        /// </summary>
        /// <param name="ids"> matchs identifier </param>
        /// <returns>Returns list of matchs</returns>
        public IList<Match> FetchMatchsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.MatchDateTime, true, _matchRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns match or null</returns>
        public Match GetMatch(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _matchRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns match or null</returns>
        public Match GetMatchFromShortLink(string shortLink, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shortLink)) throw new ArgumentNullException(nameof(shortLink));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.ShortLink == shortLink, _matchRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stats</returns>
        public IList<DivisionMatchResult> GetMatchStats(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var existingMatch = this._matchRepository.GetSingle(x => x.Id == id);

            var existingStages = this._stageRepository.Fetch(x => x.MatchId == id);

            var existingGroups = this._groupRepository.Fetch(x => x.MatchId == id);
            var existingGroupsIds = existingGroups.Select(x=>x.Id);
            var existingShooterGroups = this._groupShooterRepository.Fetch(x => existingGroupsIds.Contains(x.GroupId));

            var groupDivisions = existingShooterGroups.GroupBy(x=>x.DivisionId).Select(x=>new {
                Division = x.Key,
                Shooters = x.Select(s=> new{
                    s.ShooterId,
                    s.TeamId
                    })
            });

            var existingStageIds = existingStages.Select(x => x.Id);

            var existingShootersResult = this._shooterStageRepository.Fetch(x => existingStageIds.Contains(x.StageId));

            var shooterIds = existingShooterGroups.Select(x=>x.ShooterId).ToList();
            var existingShooters = this.FetchShootersByIds(shooterIds);

            var shooterAssociation = _shooterAssociationRepository.Fetch(x=> shooterIds.Contains(x.ShooterId) && x.AssociationId == id);

            var existingTeams = _teamRepository.Fetch();

            return groupDivisions.Select(x=> new DivisionMatchResult{
                Name = x.Division,
                StageNumber = existingStages.Select(x=>x.Name).ToList(),
                Ranks = x.Shooters.Select(s=> new ShooterMatchResult{
                    Shooter = existingShooters.FirstOrDefault(e=>e.Id == s.ShooterId),
                    TeamName = existingTeams.FirstOrDefault(e=> e.Id == s.TeamId)?.Name ?? "",
                    Rank = existingMatch.UnifyRanks ? "Unranked" : shooterAssociation.FirstOrDefault(e=> e.ShooterId == s.ShooterId)?.Rank ?? "Unranked",
                    Results = existingShootersResult.Where(e=>e.ShooterId == s.ShooterId)
                            .Select(y =>
                                new ShooterStageResult
                                {
                                    StageIndex = existingStages.First(z => z.Id == y.StageId).Index,
                                    Total= y.Total
                                }).OrderBy(y=> y.StageIndex).ToList()
                })
                .GroupBy(e=>e.Rank).Select(
                    s=> new ShooterRankResult{
                        Rank = s.Key,
                        Shooters = s.OrderBy(e=>e.Results.Sum(y=>y.Total)).ToList()
                    }
                ).ToList()
            }).ToList();
        }


        /// <summary>
        /// Create provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateMatch(Match entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Match seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // check association
            if(_associationRepository.GetSingle(x=>x.Id == entity.AssociationId) == null){
                validations.Add(new ValidationResult($"Association provided doesn`t exists"));
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckMatchValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;
      // Set unique identifier
      entity.ShortLink = RandomUtils.RandomString();
      //Esecuzione in transazione
      using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _matchRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _matchRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }

        /// <summary>
        /// Updates provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateMatch(Match entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            IList<ValidationResult> validations = new List<ValidationResult>();
            // check association
            if(_associationRepository.GetSingle(x=>x.Id == entity.AssociationId) == null){
                validations.Add(new ValidationResult($"Association provided doesn`t exists"));
                return validations;
            }
            //Predisposizione al fallimento

            // controllo singolatità emplyee
            validations = CheckMatchValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _matchRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _matchRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }


        /// <summary>
        /// Check match validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckMatchValidation(Match entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza match con stesso nome / PEC / SDI
            var existing = _matchRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteMatch(Match entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided match doesn't have valid Id");

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _matchRepository.Delete(entity);
                t.Commit();
                return new List<ValidationResult>();
            }
        }

        #endregion
#region Team

        /// <summary>
        /// Count list of all teams
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of teams</returns>
        public int CountTeams()
        {
            //Utilizzo il metodo base
            return _teamRepository.Count();
        }

        /// <summary>
        /// Fetch list of all teams
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Team> FetchAllTeams()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.Name, false, _teamRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Team> FetchTeamsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _teamRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public Team GetTeam(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _teamRepository);
        }

        /// <summary>
        /// Create provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateTeam(Team entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Team seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckTeamValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _teamRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _teamRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }

        /// <summary>
        /// Updates provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateTeam(Team entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Predisposizione al fallimento

            // controllo singolatità emplyee
            validations = CheckTeamValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _teamRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _teamRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }


        /// <summary>
        /// Check team validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckTeamValidation(Team entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza team con stesso nome / PEC / SDI
            var existing = _teamRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteTeam(Team entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided team doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

                // remove all shooterassociation for team
            var shooterTeams = _shooterTeamRepository.Fetch(x=> x.TeamId == entity.Id);

            foreach(var shooterTeam in shooterTeams){
                _shooterTeamRepository.Delete(shooterTeam);
            }

            //Eliminazione
            _teamRepository.Delete(entity);


            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion
#region Association

        /// <summary>
        /// Count list of all associations
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of associations</returns>
        public int CountAssociations()
        {
            //Utilizzo il metodo base
            return _associationRepository.Count();
        }

        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of associations</returns>
        public IList<Association> FetchAllAssociations()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.Name, false, _associationRepository);
        }

        /// <summary>
        /// Fetch list of associations by provided ids
        /// </summary>
        /// <param name="ids"> associations identifier </param>
        /// <returns>Returns list of associations</returns>
        public IList<Association> FetchAssociationsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _associationRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns association or null</returns>
        public Association GetAssociation(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _associationRepository);
        }


        /// <summary>
        /// Create provided association
        /// </summary>
        /// <param name="entity">Association</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateAssociation(Association entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Association seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckAssociationValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _associationRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _associationRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }

        /// <summary>
        /// Updates provided association
        /// </summary>
        /// <param name="entity">Association</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateAssociation(Association entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckAssociationValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _associationRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _associationRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }


        /// <summary>
        /// Check association validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckAssociationValidation(Association entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza association con stesso nome / PEC / SDI
            var existing = _associationRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided association
        /// </summary>
        /// <param name="entity">Association</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteAssociation(Association entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided association doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shooterassociation for team
            var shooterAssociations = _shooterAssociationRepository.Fetch(x=> x.AssociationId == entity.Id);

            foreach(var shooterAssociation in shooterAssociations){
                _shooterAssociationRepository.Delete(shooterAssociation);
            }

            //Eliminazione
            _associationRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region Shooter

        /// <summary>
        /// Count list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of shooters</returns>
        public int CountShooters()
        {
            //Utilizzo il metodo base
            return _shooterRepository.Count();
        }
        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchAllShooters()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, null, true, _shooterRepository);
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="ids"> shooters identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchShootersByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, x=> x.LastName, true, _shooterRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public Shooter GetShooter(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _shooterRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateShooter(Shooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Shooter seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckShooterValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _shooterRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _shooterRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateShooter(Shooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            // controllo singolatità emplyee
            IList<ValidationResult> validations = CheckShooterValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _shooterRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _shooterRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }


        /// <summary>
        /// Check shooter validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckShooterValidation(Shooter entity)
        {
            var validations = new List<ValidationResult>();

            //   // controllo esistenza shooter con stesso nome / PEC / SDI
            //   var existing = _shooterRepository.GetSingle(x => x.Id != entity.Id
            //                                                     && x.Name == entity.Name);

            //   if (existing != null)
            //   {
            //     validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            //   }

            return validations;
        }

        /// <summary>
        /// Delete provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooter(Shooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shootergroup for shooter
            var groupShooters = _groupShooterRepository.Fetch(x=> x.ShooterId == entity.Id);

            foreach(var groupShooter in groupShooters){
                _groupShooterRepository.Delete(groupShooter);
            }

            // remove all shooterassociation for shooter
            var shooterAssociations = _shooterAssociationRepository.Fetch(x=> x.ShooterId == entity.Id);

            foreach(var shooterAssociation in shooterAssociations){
                _shooterAssociationRepository.Delete(shooterAssociation);
            }

            // remove all shooterteam for shooter
            var shooterTeams = _shooterTeamRepository.Fetch(x=> x.ShooterId == entity.Id);

            foreach(var shooterTeam in shooterTeams){
                _shooterTeamRepository.Delete(shooterTeam);
            }

            //Eliminazione
            _shooterRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }

        #endregion
        #region Group

        /// <summary>
        /// Count list of all groups
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of groups</returns>
        public int CountGroups()
        {
            //Utilizzo il metodo base
            return _groupRepository.Count();
        }
        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<Group> FetchAllGroupsByMatchId(string matchId)
        {
            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, x=>x.Name, false, _groupRepository);
        }

        /// <summary>
        /// Fetch list of groups by provided ids
        /// </summary>
        /// <param name="ids"> groups identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<Group> FetchGroupsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, false, _groupRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns group or null</returns>
        public Group GetGroup(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _groupRepository);
        }


        /// <summary>
        /// Create provided group
        /// </summary>
        /// <param name="entity">Group</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateGroup(Group entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Group seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckGroupValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _groupRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _groupRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="entity">Group</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateGroup(Group entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckGroupValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _groupRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _groupRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }


        /// <summary>
        /// Check group validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckGroupValidation(Group entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza group con stesso nome / PEC / SDI
            var existing = _groupRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided group
        /// </summary>
        /// <param name="entity">Group</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteGroup(Group entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided group doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shootergroup in group
            var groupShooters = _groupShooterRepository.Fetch(x=> x.GroupId == entity.Id);

            foreach(var groupShooter in groupShooters){
                _groupShooterRepository.Delete(groupShooter);
            }

            //Eliminazione
            _groupRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region Stage

        /// <summary>
        /// Count list of all stages
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of stages</returns>
        public int CountStages()
        {
            //Utilizzo il metodo base
            return _stageRepository.Count();
        }
        /// <summary>
        /// Fetch list of all stages
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of stages</returns>
        public IList<Stage> FetchAllStages(string matchId)
        {
            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, null, true, _stageRepository);
        }

        /// <summary>
        /// Fetch list of stages by provided ids
        /// </summary>
        /// <param name="ids"> stages identifier </param>
        /// <returns>Returns list of stages</returns>
        public IList<Stage> FetchStagesByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _stageRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public Stage GetStage(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _stageRepository);
        }


        /// <summary>
        /// Create provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateStage(Stage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Stage seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckStageValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _stageRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _stageRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }

        /// <summary>
        /// Updates provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateStage(Stage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckStageValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                validations = _stageRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _stageRepository.Save(entity);
                t.Commit();
                return validations;
            }
        }


        /// <summary>
        /// Check stage validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckStageValidation(Stage entity)
        {
            var validations = new List<ValidationResult>();

            //   // controllo esistenza stage con stesso nome / PEC / SDI
            var existing = _stageRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteStage(Stage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _stageRepository.Delete(entity);
                t.Commit();
                return new List<ValidationResult>();
            }
        }
        #endregion
        #region shooterstage
        /// <summary>
        /// Fetch list of all stage
        /// </summary>
        /// <param name="matchId"> match identifier </param>
        /// <returns>Returns list of stages</returns>
        public IList<Stage> FetchAllStagesByMatchId(string matchId)
        {
            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, x=>x.Index, false, _stageRepository);
        }


        /// <summary>
        /// fetch shooters results on stage
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public IList<ShooterStage> FetchShootersResultsOnStage(string stageId, IList<string> shooterIds)
        {
            return FetchEntities(e => e.StageId == stageId && shooterIds.Contains(e.ShooterId), null, null, null, true, _shooterStageRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterStage(ShooterStage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            IList<ValidationResult> validations = new List<ValidationResult>();

            var existingStage = this._stageRepository.GetSingle(x => entity.StageId == x.Id);

            if (existingStage == null)
            {
                validations.Add(new ValidationResult($"{nameof(existingStage)} not found"));
                return validations;
            }

            // point check
            if (existingStage.Targets != entity.DownPoints.Count)
            {
                validations.Add(new ValidationResult($"Stage points and downPoint reported are missmatching"));
                return validations;
            }

            var existingShooterStage = this._shooterStageRepository.GetSingle(x => x.ShooterId == entity.ShooterId && entity.StageId == x.StageId);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // new element
            if (existingShooterStage == null)
            {
                // Settaggio data di creazione
                entity.CreationDateTime = DateTime.UtcNow;

                //Validazione argomenti
                validations = _shooterStageRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _shooterStageRepository.Save(entity);
                t.Commit();
                return validations;
            }

            existingShooterStage.Time = entity.Time;
            existingShooterStage.DownPoints = entity.DownPoints;
            existingShooterStage.Procedurals = entity.Procedurals;
            existingShooterStage.HitOnNonThreat = entity.HitOnNonThreat;
            existingShooterStage.FlagrantPenalties = entity.FlagrantPenalties;
            existingShooterStage.Ftdr = entity.Ftdr;
            existingShooterStage.Procedural = entity.Procedural;
            existingShooterStage.Disqualified = entity.Disqualified;

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (existingShooterStage.CreationDateTime < new DateTime(2000, 1, 1))
                existingShooterStage.CreationDateTime = new DateTime(2000, 1, 1);

            //Validazione argomenti
            validations = _shooterStageRepository.Validate(existingShooterStage);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterStageRepository.Save(existingShooterStage);
            t.Commit();
            return validations;
        }

        #endregion
        #region groupshooter




        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchShootersByGroupId(string id)
        {
            var shooterIds = this._groupShooterRepository.Fetch(x=>x.GroupId == id).Select(x=> x.ShooterId);
            //Utilizzo il metodo base
            return FetchEntities(s => shooterIds.Contains(s.Id), null, null, x=>x.LastName, false, _shooterRepository);
        }
        /// <summary>
        /// Fetch available shooter for group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<Shooter> FetchAvailableShooters(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            // retrieve match for the group
            var match = this._matchRepository.GetSingle(x=> x.Id == group.MatchId);

            if (match == null) throw new ArgumentNullException(nameof(match));

            // find group in the same match
            var groupInMatchIds = this._groupRepository.Fetch(x => x.MatchId == group.MatchId).Select(x=>x.Id).ToList();

            // find shooter in other groups
            var unAvailableUsers = this._groupShooterRepository
                                                    .Fetch(x => groupInMatchIds.Contains(x.GroupId))
                                                    .Select(x => x.ShooterId)
                                                    .ToList();

            // retrieve shooter in same association
            var shooterInTeamAssociation = this._shooterAssociationRepository.Fetch(x=>x.AssociationId== match.AssociationId).Select(x=>x.ShooterId).ToList();

            // retrieve shooter not from available user and in association
            return this._shooterRepository.Fetch(x => !unAvailableUsers.Contains(x.Id) && (match.OpenMatch || shooterInTeamAssociation.Contains(x.Id)),null,null,x=>x.LastName,false);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public GroupShooter GetGroupShooterByShooterAndGroup(string shooterId, string groupId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(groupId)) throw new ArgumentNullException(nameof(groupId));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.ShooterId == shooterId && c.GroupId == groupId, _groupShooterRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertGroupShooter(GroupShooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _groupShooterRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _groupShooterRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteGroupShooter(GroupShooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _groupShooterRepository.Delete(entity);
                t.Commit();
                    return new List<ValidationResult>();
            }
        }
        #endregion
        #region shooterteam

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public ShooterTeam GetShooterTeamByTeamAndShooterId(string TeamId,string ShooterId)
        {
            return this._shooterTeamRepository.GetSingle(x=>x.TeamId == TeamId && x.ShooterId == ShooterId);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterTeam> FetchTeamsFromShooterId(string shooterId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));

            return FetchEntities(e => e.ShooterId == shooterId, null, null, null, true, _shooterTeamRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterTeam> FetchShootersFromTeamId(string teamId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(teamId)) throw new ArgumentNullException(nameof(teamId));
            return FetchEntities(e => e.TeamId == teamId, null, null, null, true, _shooterTeamRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="teamId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterTeam(ShooterTeam entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _shooterTeamRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterTeamRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterTeam(ShooterTeam entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _shooterTeamRepository.Delete(entity);
                t.Commit();
                return new List<ValidationResult>();
            }
        }
        #endregion

        #region shooterassociation

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterAssociation> FetchShooterAssociationByShooterId(string shooterId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));

            return FetchEntities(e => e.ShooterId == shooterId, null, null, null, true, _shooterAssociationRepository);
        }
        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public ShooterAssociation GetShooterAssociationByShooterAndAssociation(string shooterId, string associationId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(associationId)) throw new ArgumentNullException(nameof(associationId));

            return FetchEntities(c => c.ShooterId == shooterId && c.AssociationId == associationId, null, null, x=>x.RegistrationDate, true, _shooterAssociationRepository).FirstOrDefault();
        }

        /// <summary>
        /// Updates provided association
        /// </summary>
        /// <param name="associationId">association id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterAssociation(ShooterAssociation entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _shooterAssociationRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterAssociationRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterAssociation(ShooterAssociation entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _shooterAssociationRepository.Delete(entity);
                t.Commit();
                    return new List<ValidationResult>();
            }
        }
        #endregion

#region NotificationSubscription

        /// <summary>
        /// Fetch list of notificationsubscriptions by provided ids
        /// </summary>
        /// <param name="ids"> notificationsubscriptions identifier </param>
        /// <returns>Returns list of notificationsubscriptions</returns>
        public IList<NotificationSubscription> FetchNotificationSubscriptionsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _notificationsubscriptionRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns notificationsubscription or null</returns>
        public NotificationSubscription GetNotificationSubscription(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _notificationsubscriptionRepository);
}

/// <summary>
        /// Create provided notificationsubscription
        /// </summary>
        /// <param name="entity">NotificationSubscription</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateNotificationSubscription(NotificationSubscription entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided NotificationSubscription seems to already existing");

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using (var t = DataSession.BeginTransaction())
            {

                //Validazione argomenti
                var validations = _notificationsubscriptionRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _notificationsubscriptionRepository.Save(entity);
                t.Commit();
                return validations;
            }
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
                _groupRepository.Dispose();
                _groupShooterRepository.Dispose();
                _matchRepository.Dispose();
                _associationRepository.Dispose();
                _shooterRepository.Dispose();
                _shooterStageRepository.Dispose();
                _stageRepository.Dispose();
                _teamRepository.Dispose();
        _notificationsubscriptionRepository.Dispose();
      }

            //Chiamo il metodo base
            base.Dispose(isDisposing);
        }
    }
}