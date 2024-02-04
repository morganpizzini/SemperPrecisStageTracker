using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooter
    /// </summary>
    [ApiVersion("1.0")]
    [Obsolete]
    public class ShooterController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllShooters")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> FetchAllShooters()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllUsers();
            var shooterIds = entities.Select(x=>x.Id).ToList();

            var shooterDatas = BasicLayer.FetchUserDataByUserIds(shooterIds);


            //Ritorno i contratti
            return Reply(entities.As(x => ContractUtils.GenerateContract(x,shooterDatas.FirstOrDefault(y=>y.UserId == x.Id),null,null,false)));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetShooter")]
        [ProducesResponseType(typeof(UserContract), 200)]
        public Task<IActionResult> GetShooter([FromBody]ShooterRequest request)
        {
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            var data = BasicLayer.GetUserData(entity.Id);

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,data));
        }

        /// <summary>
        /// Creates a shooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateShooter")]
        [ApiAuthorizationFilter(Permissions.ManageUsers, Permissions.CreateUser)]
        [ProducesResponseType(typeof(UserContract), 200)]
        public async Task<IActionResult> CreateShooter([FromBody]UserCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                Email = request.Email,
                Username = request.Username
            };
            var data = new UserData
            {
                FirearmsLicence = request.FirearmsLicence,
                FirearmsLicenceExpireDate = request.FirearmsLicenceExpireDate,
                FirearmsLicenceReleaseDate = request.FirearmsLicenceReleaseDate,
                MedicalExaminationExpireDate = request.MedicalExaminationExpireDate,
                BirthLocation = request.BirthLocation,
                Address = request.Address,
                City = request.City,
                PostalCode = request.PostalCode,
                Province = request.Province,
                Country = request.Country,
                Phone = request.Phone,
                FiscalCode = request.FiscalCode
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateUser(model, data, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model,data));
        }

        /// <summary>
        /// Updates existing shooter
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateShooter")]
        [ApiAuthorizationFilter(Permissions.EditUser ,Permissions.ManageUsers )]
        [ProducesResponseType(typeof(UserContract), 200)]
        public async Task<IActionResult> UpdateShooter([FromBody]UserUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);
            var data = BasicLayer.GetUserData(request.ShooterId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null || data == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.BirthDate = request.BirthDate;
            entity.Email = request.Email;
            entity.Username = request.Username;
            data.FirearmsLicenceExpireDate = request.FirearmsLicenceExpireDate;
            data.FirearmsLicenceReleaseDate = request.FirearmsLicenceReleaseDate;
            data.FirearmsLicence = request.FirearmsLicence;
            data.MedicalExaminationExpireDate = request.MedicalExaminationExpireDate;
            data.BirthLocation = request.BirthLocation;
            data.Address = request.Address;
            data.City = request.City;
            data.PostalCode = request.PostalCode;
            data.Province = request.Province;
            data.Country = request.Country;
            data.Phone = request.Phone;
            data.FiscalCode = request.FiscalCode;

            //Salvataggio
            var validations = await BasicLayer.UpdateUser(entity,data, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity,data));
        }

        /// <summary>
        /// Deletes existing shooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooter")]
        [ApiAuthorizationFilter(Permissions.ManageUsers,Permissions.UserDelete )]
        [ProducesResponseType(typeof(UserContract), 200)]
        public async Task<IActionResult> DeleteShooter([FromBody]ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteUser(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
