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
    public int CountFidelityCardTypes()
    {
        //Utilizzo il metodo base
        return _fidelityCardTypeRepository.Count();
    }

    /// <summary>
    /// Fetch list of all places
    /// </summary>
    /// <param name="userId"> user identifier </param>
    /// <returns>Returns list of places</returns>
    public IList<FidelityCardType> FetchAllFidelityCardTypes(string placeId) => FetchAllFidelityCardTypes(new List<string> { placeId});

    public IList<FidelityCardType> FetchAllFidelityCardTypes(IList<string> placeIds)
    {
        //Utilizzo il metodo base
        return FetchEntities(x=>placeIds.Contains(x.PlaceId), null, null, s => s.Name, false, _fidelityCardTypeRepository);
    }

    /// <summary>
    /// Fetch list of places by provided ids
    /// </summary>
    /// <param name="ids"> places identifier </param>
    /// <returns>Returns list of places</returns>
    public IList<FidelityCardType> FetchFidelityCardTypesByIds(IList<string> ids)
    {
        //Utilizzo il metodo base
        return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _fidelityCardTypeRepository);
    }

    /// <summary>
    /// Get place by commissionDrawingId
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="userId">filter by userId</param>
    /// <returns>Returns place or null</returns>
    public FidelityCardType GetFidelityCardType(string id, string userId = null)
    {
        //Validazione argomenti
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

        //Utilizzo il metodo base
        return GetSingleEntity(c => c.Id == id, _fidelityCardTypeRepository);
    }

    /// <summary>
    /// Create provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> CreateFidelityCardType(FidelityCardType entity, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (!string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided FidelityCardType seems to already existing");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(CreateFidelityCardType)}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckFidelityCardTypeValidation(entity);
        if (validations.Count > 0)
        {
            return validations;
        }

        // Settaggio data di creazione
        entity.CreationDateTime = DateTime.UtcNow;

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();
        //Validazione argomenti
        validations = _fidelityCardTypeRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _fidelityCardTypeRepository.Save(entity);

        t.Commit();
        return validations;
    }

    /// <summary>
    /// Updates provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> UpdateFidelityCardType(FidelityCardType entity, string userId)
    {
        //TODO: sistemare permessi
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � nuovo, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException($"Provided fidelityCardType is new. Use {nameof(CreateFidelityCardType)}");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateFidelityCardType)} with place id: {entity.PlaceId}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckFidelityCardTypeValidation(entity);
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
        validations = _fidelityCardTypeRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _fidelityCardTypeRepository.Save(entity);


        t.Commit();

        return validations;
    }


    /// <summary>
    /// Check place validations
    /// </summary>
    /// <param name="entity">entity to check</param>
    /// <returns>List of validation results</returns>
    private IList<ValidationResult> CheckFidelityCardTypeValidation(FidelityCardType entity)
    {
        var validations = new List<ValidationResult>();

        // controllo esistenza place con stesso nome / PEC / SDI
        var existing = _fidelityCardTypeRepository.Fetch(x => x.Id != entity.Id
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
    public async Task<IList<ValidationResult>> DeleteFidelityCardType(FidelityCardType entity, string userId)
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
            validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteFidelityCardType)} with place id: {entity.PlaceId}");
            return validations;
        }

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();

        //Eliminazione
        _fidelityCardTypeRepository.Delete(entity);

        t.Commit();
        return new List<ValidationResult>();

    }
}