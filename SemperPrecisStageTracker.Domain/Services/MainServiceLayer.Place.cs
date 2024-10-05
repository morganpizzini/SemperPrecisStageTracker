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
    public int CountPlaces()
    {
        //Utilizzo il metodo base
        return _placeRepository.Count();
    }
    public IList<Place> FetchAllPlaces()
    {
        //Utilizzo il metodo base
        return FetchEntities(null, null, null, s => s.Name, false, _placeRepository);
    }
    public IList<PlaceData> FetchAllMinimunPlacesData()
    {
        //Utilizzo il metodo base
        return _placeDataRepository.FetchWithProjection(x => new PlaceData { Address = x.Address, PlaceId = x.PlaceId, Holder = x.Holder });

    }

    public IList<Place> FetchPlacesByIds(IList<string> ids)
    {
        //Utilizzo il metodo base
        return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _placeRepository);
    }

    public Place GetPlace(string id, string userId = null)
    {
        //Validazione argomenti
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

        //Utilizzo il metodo base
        return GetSingleEntity(c => c.Id == id, _placeRepository);
    }

    public PlaceData GetPlaceData(string id, string userId = null)
    {
        //Validazione argomenti
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

        //Utilizzo il metodo base
        return GetSingleEntity(c => c.PlaceId == id, _placeDataRepository);
    }

    public async Task<IList<ValidationResult>> CreatePlace(Place entity, PlaceData data, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (data == null) throw new ArgumentNullException(nameof(data));

        //Se l'oggetto � esistente, eccezione
        if (!string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided Place seems to already existing");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.CreatePlaces.ManagePlaces))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(CreatePlace)}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckPlaceValidation(entity, data);
        if (validations.Count > 0)
        {
            return validations;
        }

        // Settaggio data di creazione
        entity.CreationDateTime = DateTime.UtcNow;
        data.CreationDateTime = DateTime.UtcNow;

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();
        //Validazione argomenti
        validations = _placeRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _placeRepository.Save(entity);

        data.PlaceId = entity.Id;

        //Validazione argomenti
        validations = _placeDataRepository.Validate(data);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _placeDataRepository.Save(data);

        //Add user permission on match
        validations = await AddPlacePermission(entity.Id, userId);

        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        t.Commit();
        return validations;
    }

    public Task<IList<ValidationResult>> AddPlacePermission(string placeId, string userId)
        => AddUserPermissions(placeId, PermissionCtor.EditPlace, userId);
    
    public async Task<IList<Place>> FetchUserPlaces(string userId)
    {
        if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

        IList<ValidationResult> validations = new List<ValidationResult>();

        IList<UserPermission> newPermissions = new List<UserPermission>();

        var userPermissions = await authenticationService.GetUserPermissionByUserId(userId);
        var placeIds = userPermissions.EntityPermissions
                    .Where(x =>
                        !string.IsNullOrEmpty(x.EntityId) && x.Permissions.Contains(Permissions.EditPlace))
                    .Select(x=>x.EntityId).ToList();
        return _placeRepository.Fetch(x=> placeIds.Contains(x.Id));
    }


    public async Task<IList<ValidationResult>> UpdatePlace(Place entity, PlaceData data, string userId)
    {
        //TODO: sistemare permessi
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � nuovo, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManagePlaces.EditPlace))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(UpdatePlace)} with Id: {entity.Id}");
            return validations;
        }

        // controllo singolatità emplyee
        validations = CheckPlaceValidation(entity, data);
        if (validations.Count > 0)
        {
            return validations;
        }

        //Compensazione: se non ho la data di creazione, metto una data fittizia
        if (entity.CreationDateTime < new DateTime(2000, 1, 1))
            entity.CreationDateTime = new DateTime(2000, 1, 1);

        if (data.CreationDateTime < new DateTime(2000, 1, 1))
            data.CreationDateTime = new DateTime(2000, 1, 1);

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();
        //Validazione argomenti
        validations = _placeRepository.Validate(entity);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _placeRepository.Save(entity);

        //Validazione argomenti
        validations = _placeDataRepository.Validate(data);

        //Se ho validazioni fallite, esco
        if (validations.Count > 0)
        {
            //Rollback ed uscita
            t.Rollback();
            return validations;
        }

        //Salvataggio
        _placeDataRepository.Save(data);
        t.Commit();

        return validations;
    }

    private IList<ValidationResult> CheckPlaceValidation(Place entity, PlaceData data)
    {
        var validations = new List<ValidationResult>();

        // controllo esistenza place con stesso nome / PEC / SDI
        var existing = _placeRepository.Fetch(x => x.Id != entity.Id
                                                          && x.Name == entity.Name);

        if (existing.Count == 0)
            return validations;

        var existingIds = existing.Select(x => x.Id).ToList();

        var singlePlace = _placeDataRepository.Fetch(x =>
            existingIds.Contains(x.PlaceId) && (x.City == data.City || x.PostalCode == data.PostalCode));

        if (singlePlace.Count > 0)
        {
            validations.Add(new ValidationResult($"Entity with name {entity.Name} and same city/postal code already exists"));
        }

        return validations;
    }

    public async Task<IList<ValidationResult>> DeletePlace(Place entity, string userId)
    {
        //Validazione argomenti
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        //Se l'oggetto � esistente, eccezione
        if (string.IsNullOrEmpty(entity.Id))
            throw new InvalidProgramException("Provided place doesn't have valid Id");

        //Predisposizione al fallimento
        IList<ValidationResult> validations = new List<ValidationResult>();

        //Check permissions
        if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManagePlaces))
        {
            validations.AddMessage($"User {userId} has no permissions on {nameof(DeletePlace)} with Id: {entity.Id}");
            return validations;
        }

        //Esecuzione in transazione
        using var t = DataSession.BeginTransaction();

        // remove shooterData
        var placeData = _placeDataRepository.GetSingle(x => x.PlaceId == entity.Id);

        if (placeData != null)
            _placeDataRepository.Delete(placeData);

        //remove bays
        var bays = _bayRepository.Fetch(x => x.PlaceId == entity.Id);
        foreach (var bay in bays)
        {
            _bayRepository.Delete(bay);
        }

        //Eliminazione
        _placeRepository.Delete(entity);

        validations = await RemoveUserValidation(entity.Id, PermissionCtor.EditPlace);
        if (validations.Count > 1)
        {
            t.Rollback();
            return validations;
        }

        t.Commit();
        return new List<ValidationResult>();

    }

}