using SemperPrecisStageTracker.Domain.Utils;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Domain.Services;

public partial class MainServiceLayer
{
    public IList<Schedule> FetchAllBaySchedules(string bayId) => FetchAllBaySchedules(new List<string> { bayId });

    public IList<Schedule> FetchAllBaySchedules(IList<string> bayIds)
    {
        var scheduleIds = FetchEntities(x => bayIds.Contains(x.BayId), null, null, null, false, _bayScheduleRepository)
            .Select(x=>x.ScheduleId)
            .ToList();

        //Utilizzo il metodo base
        return FetchEntities(x=> scheduleIds.Contains(x.Id), null, null, s => s.Name, false, _scheduleRepository);
    }

    public BaySchedule GetBaySchedule(string bayId,string scheduleId, string userId = null)
    {
        //Validazione argomenti
        if (string.IsNullOrEmpty(bayId)) throw new ArgumentNullException(nameof(bayId));
        if (string.IsNullOrEmpty(scheduleId)) throw new ArgumentNullException(nameof(scheduleId));

        //Utilizzo il metodo base
        return GetSingleEntity(c => c.BayId == bayId && c.ScheduleId == scheduleId, _bayScheduleRepository);
    }

    /// <summary>
    /// Create provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> CreateBaySchedule(BaySchedule entity, string placeId, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (!string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided Bay seems to already existing");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, placeId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(CreateBaySchedule)}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckBayScheduleValidation(entity);
        if (validations.Count > 0)
        {
            return validations;
        }

        // Settaggio data di creazione
        entity.CreationDateTime = DateTime.UtcNow;

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();
        //Validazione argomenti
        validations = _bayScheduleRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _bayScheduleRepository.Save(entity);

        t.Commit();
        return validations;
    }

    
    /// <summary>
    /// Check place validations
    /// </summary>
    /// <param name="entity">entity to check</param>
    /// <returns>List of validation results</returns>
    private IList<ValidationResult> CheckBayScheduleValidation(BaySchedule entity)
    {
        var validations = new List<ValidationResult>();

        // controllo esistenza place con stesso nome / PEC / SDI
        var existing = _bayScheduleRepository.Fetch(x => x.Id != entity.Id
                                                && x.BayId == entity.BayId
                                                          && x.ScheduleId == entity.ScheduleId);

        if (existing.Count > 0)
            validations.Add(new ValidationResult($"Entity with bay id {entity.BayId} and schedule id {entity.ScheduleId} already exists"));

        return validations;
    }

    /// <summary>
    /// Delete provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> DeleteBaySchedule(BaySchedule entity, string placeId, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided bay schedule doesn't have valid Id");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, placeId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteBaySchedule)} with place id: {placeId}");
            return validations;
        }

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();

        //Eliminazione
        _bayScheduleRepository.Delete(entity);

        t.Commit();
        return new List<ValidationResult>();

    }
}