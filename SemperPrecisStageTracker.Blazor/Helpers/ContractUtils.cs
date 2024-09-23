using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Helpers;

public static class ContractUtils
{
    public static UserCreateRequest Convert(UserContract model) =>
        new UserCreateRequest()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            BirthDate = model.BirthDate,
            Username = model.Username,
            Email = model.Email,
            FirearmsLicenceExpireDate = model.FirearmsLicenceExpireDate,
            FirearmsLicence = model.FirearmsLicence,
            MedicalExaminationExpireDate = model.MedicalExaminationExpireDate,
            FirearmsLicenceReleaseDate = model.FirearmsLicenceReleaseDate,
            BirthLocation = model.BirthLocation,
            Address = model.Address,
            City = model.City,
            PostalCode = model.PostalCode,
            Province = model.Region,
            Country = model.Country,
            Phone = model.Phone,
            FiscalCode = model.FiscalCode,
            Gender = model.Gender
        };
}
