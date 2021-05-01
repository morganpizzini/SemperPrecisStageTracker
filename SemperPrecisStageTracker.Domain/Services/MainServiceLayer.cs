using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.ServicesLayers;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class MainServiceLayer : DataServiceLayerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IShooterRepository _shooterRepository;
        private readonly IShooterStageRepository _shooterStageRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IGroupShooterRepository _groupShooterRepository;

        public MainServiceLayer(IDataSession dataSession)
            : base(dataSession)
        {
            _groupRepository = dataSession.ResolveRepository<IGroupRepository>();
            _groupShooterRepository = dataSession.ResolveRepository<IGroupShooterRepository>();
            _matchRepository = dataSession.ResolveRepository<IMatchRepository>();
            _shooterRepository = dataSession.ResolveRepository<IShooterRepository>();
            _shooterStageRepository = dataSession.ResolveRepository<IShooterStageRepository>();
            _stageRepository = dataSession.ResolveRepository<IStageRepository>();
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
        /// <returns>Returns stats</returns>
        public IList<ShooterMatchResult> GetMatchStats(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var existingStages = this._stageRepository.Fetch(x => x.MatchId == id);

            var existingStageIds = existingStages.Select(x => x.Id);

            var existingShootersResult = this._shooterStageRepository.Fetch(x => existingStageIds.Contains(x.StageId));

            var existingShooters = this.FetchShootersByIds(existingShootersResult.Select(x=>x.ShooterId).ToList());

            return existingShootersResult.GroupBy(x => x.ShooterId).Select(x => new ShooterMatchResult
            {
                Shooter = existingShooters.First(y=> y.Id == x.Key),
                Results = x.Select(y =>
                    new ShooterStageResult
                    {
                        StageIndex = existingStages.First(z => z.Id == y.StageId).Index,
                        Total= y.Total
                    }).OrderBy(y=> y.StageIndex).ToList()
            }).OrderBy(x=>x.Results.Sum(y=>y.Total)).ToList();
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

            // controllo singolatità emplyee
            validations = CheckMatchValidation(entity);
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

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

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
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _shooterRepository);
        }


        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchShootersByGroupId(string id)
        {
            var shooterIds = this._groupShooterRepository.Fetch(x=>x.GroupId == id).Select(x=> x.ShooterId);
            //Utilizzo il metodo base
            return FetchEntities(s => shooterIds.Contains(s.Id), null, null, null, true, _shooterRepository);
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

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckShooterValidation(entity);
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
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _shooterRepository.Delete(entity);
                t.Commit();
                return new List<ValidationResult>();
            }
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
        public IList<Group> FetchAllGroups(string matchId)
        {
            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, null, true, _groupRepository);
        }

        /// <summary>
        /// Fetch list of groups by provided ids
        /// </summary>
        /// <param name="ids"> groups identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<Group> FetchGroupsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _groupRepository);
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
            using (var t = DataSession.BeginTransaction())
            {
                //Eliminazione
                _groupRepository.Delete(entity);
                t.Commit();
                return new List<ValidationResult>();
            }
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

            if (existingShooterStage == null)
            {
                // new element

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

            // update existing
            existingShooterStage.DownPoints = entity.DownPoints;
            existingShooterStage.Penalties = entity.Penalties;
            existingShooterStage.Procedures = entity.Procedures;
            existingShooterStage.FailureToNeutralize = entity.FailureToNeutralize;
            existingShooterStage.MissedEngagement = entity.MissedEngagement;
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
        #region shootergroup

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShootersInGroup(string groupId, IList<string> shooterIds)
        {
            //Validazione argomenti
            if (groupId == null) throw new ArgumentNullException(nameof(groupId));
            IList<ValidationResult> validations = new List<ValidationResult>();

            var existingGroup = this.GetGroup(groupId);

            //Se l'oggetto � nuovo, eccezione
            if (existingGroup == null)
            {
                validations.Add(new ValidationResult($"{nameof(existingGroup)} not found"));
                return validations;
            }

            var entities = this._groupShooterRepository.Fetch(x => x.GroupId == existingGroup.Id);
            var existingGroupShooterIds = entities.Select(x => x.ShooterId);

            var existingShooters = this.FetchShootersByIds(shooterIds);
            var existingShooterIds = existingShooters.Select(x => x.Id);

            var newShooters = existingShooterIds.Where(x => !existingGroupShooterIds.Contains(x));

            var creationDatetime = DateTime.Now;
            using var t = DataSession.BeginTransaction();

            foreach (var shooterId in newShooters)
            {

                var groupShooter = new GroupShooter
                {
                    GroupId = existingGroup.Id,
                    ShooterId = shooterId,
                    CreationDateTime = creationDatetime
                };
                validations = _groupShooterRepository.Validate(groupShooter);
                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }
                //Salvataggio
                _groupShooterRepository.Save(groupShooter);
            }


            var deletedEntities = entities.Where(x => !existingShooterIds.Contains(x.ShooterId)).ToList();

            //Eliminazione
            foreach (var groupShooter in deletedEntities)
            {
                _groupShooterRepository.Delete(groupShooter);
            }

            t.Commit();
            return validations;
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
                _shooterRepository.Dispose();
                _shooterStageRepository.Dispose();
                _stageRepository.Dispose();
            }

            //Chiamo il metodo base
            base.Dispose(isDisposing);
        }
    }
}