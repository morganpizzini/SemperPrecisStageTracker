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
    /// Fetch list of all places
    /// </summary>
    /// <param name="userId"> user identifier </param>
    /// <returns>Returns list of places</returns>
    public IList<Reservation> FetchAllReservations(string bayId) => FetchAllReservations(new List<string> { bayId});

    public IList<Reservation> FetchAllReservations(IList<string> bayIds)
    {
        //Utilizzo il metodo base
        return FetchEntities(x=>bayIds.Contains(x.BayId), null, null, s => s.Day, false, _reservationRepository);
    }

    /// <summary>
    /// Fetch list of places by provided ids
    /// </summary>
    /// <param name="ids"> places identifier </param>
    /// <returns>Returns list of places</returns>
    public IList<Reservation> FetchReservationsByIds(IList<string> ids)
    {
        //Utilizzo il metodo base
        return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Day, false, _reservationRepository);
    }

    /// <summary>
    /// Get place by commissionDrawingId
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="userId">filter by userId</param>
    /// <returns>Returns place or null</returns>
    public Reservation GetReservation(string id, string userId = null)
    {
        //Validazione argomenti
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

        //Utilizzo il metodo base
        return GetSingleEntity(c => c.Id == id, _reservationRepository);
    }

    /// <summary>
    /// Create provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> CreateReservation(Reservation entity, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (!string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided Reservation seems to already existing");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        ////Check permissions
        //if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        //{
        //    validations.AddMessage($"User {userId} has no permissions on {nameof(CreateReservation)}");
        //    return validations;
        //}

        // controllo singolatità emplyee
        validations = CheckReservationValidation(entity);
        if (validations.Count > 0)
        {
            return validations;
        }

        // Settaggio data di creazione
        entity.CreationDateTime = DateTime.UtcNow;

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();
        //Validazione argomenti
        validations = _reservationRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _reservationRepository.Save(entity);

        t.Commit();
        return validations;
    }

    /// <summary>
    /// Updates provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> UpdateReservation(Reservation entity, string userId)
    {
        //TODO: sistemare permessi
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � nuovo, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException($"Provided reservation is new. Use {nameof(CreateReservation)}");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        ////Check permissions
        //if (!await authenticationService.ValidateUserPermissions(userId, entity.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        //{
        //    validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateReservation)} with place id: {entity.PlaceId}");
        //    return validations;
        //}

        // controllo singolatità emplyee
        validations = CheckReservationValidation(entity);
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
        validations = _reservationRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _reservationRepository.Save(entity);

        t.Commit();

        return validations;
    }


    /// <summary>
    /// Check place validations
    /// </summary>
    /// <param name="entity">entity to check</param>
    /// <returns>List of validation results</returns>
    private IList<ValidationResult> CheckReservationValidation(Reservation entity)
    {
        var validations = new List<ValidationResult>();

        //check lock

        var existing = _reservationRepository.Fetch(x => 
                                                          string.IsNullOrEmpty(x.UserId)
                                                          && x.From <= entity.From
                                                          && x.To >= entity.To
                                                          && x.Day == entity.Day);

        if (existing.Count > 0)
            validations.Add(new ValidationResult($"Reservation has been locked for day {entity.Day:g}, from {entity.From} to {entity.To}"));

        // controllo esistenza place con stesso nome / PEC / SDI
        existing = _reservationRepository.Fetch(x => x.Id != entity.Id
                                                && x.UserId == entity.UserId      
                                                          && x.From <= entity.From
                                                          && x.To >= entity.To
                                                          && x.Day == entity.Day);

        if (existing.Count > 0)
            validations.Add(new ValidationResult($"Reservation for {entity.UserId} already exists"));

        return validations;
    }

    /// <summary>
    /// Delete provided place
    /// </summary>
    /// <param name="entity">Place</param>
    /// <returns>Returns list of validations</returns>
    public async Task<IList<ValidationResult>> DeleteReservation(Reservation entity, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided place doesn't have valid Id");

        //get bay from reservation
        var bay = GetBay(entity.BayId);

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        if(bay == null)
        {
            validations.AddMessage($"Bay form reservation not found");
            return validations;
        }

        //Check permissions
        if (entity.UserId != userId && !await authenticationService.ValidateUserPermissions(userId, bay.PlaceId, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteReservation)} with place id: {bay.PlaceId}");
            return validations;
        }

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();

        //Eliminazione
        _reservationRepository.Delete(entity);

        t.Commit();
        return new List<ValidationResult>();

    }
}