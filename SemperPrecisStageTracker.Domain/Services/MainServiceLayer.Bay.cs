using SemperPrecisStageTracker.Domain.Utils;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Domain.Services;

public partial class MainServiceLayer
{
    /// <summary>
    /// Count list of all places
    /// </summary>
    /// <param name="userId"> user identifier </param>
    /// <returns>Returns number of places</returns>
    public int CountBays()
    {
        //Utilizzo il metodo base
        return _bayRepository.Count();
    }

    /// <summary>
    /// Fetch list of all places
    /// </summary>
    /// <param name="userId"> user identifier </param>
    /// <returns>Returns list of places</returns>
    public IList<Bay> FetchAllBays(string placeId) => FetchAllBays(new List<string> { placeId});

    public IList<Bay> FetchAllBays(IList<string> placeIds)
    {
        //Utilizzo il metodo base
        return FetchEntities(x=>placeIds.Contains(x.PlaceId), null, null, s => s.Name, false, _bayRepository);
    }

    /// <summary>
    /// Fetch list of places by provided ids
    /// </summary>
    /// <param name="ids"> places identifier </param>
    /// <returns>Returns list of places</returns>
    public IList<Bay> FetchBaysByIds(IList<string> ids)
    {
        //Utilizzo il metodo base
        return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _bayRepository);
    }

    /// <summary>
    /// Get place by commissionDrawingId
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="userId">filter by userId</param>
    /// <returns>Returns place or null</returns>
    public Bay GetBay(string id, string userId = null)
    {
        //Validazione argomenti
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

        //Utilizzo il metodo base
        return GetSingleEntity(c => c.Id == id, _bayRepository);
    }

    /// <summary>
    /// Create provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> CreateBay(Bay entity, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (!string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided Bay seems to already existing");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(CreateBay)}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckBayValidation(entity);
        if (validations.Count > 0)
        {
            return validations;
        }

        // Settaggio data di creazione
        entity.CreationDateTime = DateTime.UtcNow;

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();
        //Validazione argomenti
        validations = _bayRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _bayRepository.Save(entity);

        t.Commit();
        return validations;
    }

    /// <summary>
    /// Updates provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> UpdateBay(Bay entity, string userId)
    {
        //TODO: sistemare permessi
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � nuovo, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException($"Provided bay is new. Use {nameof(CreateBay)}");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateBay)} with place id: {entity.PlaceId}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckBayValidation(entity);
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
        validations = _bayRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _bayRepository.Save(entity);


        t.Commit();

        return validations;
    }


    /// <summary>
    /// Check place validations
    /// </summary>
    /// <param name="entity">entity to check</param>
    /// <returns>List of validation results</returns>
    private IList<ValidationResult> CheckBayValidation(Bay entity)
    {
        var validations = new List<ValidationResult>();

        // controllo esistenza place con stesso nome / PEC / SDI
        var existing = _bayRepository.Fetch(x => x.Id != entity.Id
                                                && x.PlaceId == entity.PlaceId      
                                                          && x.Name == entity.Name);

        if (existing.Count > 0)
            validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));

        return validations;
    }

    /// <summary>
    /// Delete provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> DeleteBay(Bay entity, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided place doesn't have valid Id");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteBay)} with place id: {entity.PlaceId}");
            return validations;
        }

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();

        //Eliminazione
        _bayRepository.Delete(entity);

        t.Commit();
        return new List<ValidationResult>();

    }
}