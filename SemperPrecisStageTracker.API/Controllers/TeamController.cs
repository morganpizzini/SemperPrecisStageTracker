using System.Collections.Generic;
using System.Linq;
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
    /// Controller for team
    /// </summary>
    public class TeamController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all teams
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllTeams")]
        [ProducesResponseType(typeof(IList<TeamContract>), 200)]
        public Task<IActionResult> FetchAllTeams()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllTeams();

            //Ritorno i contratti
            return Reply(entities.As(ContractUtils.GenerateContract));
        }
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetTeam")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public Task<IActionResult> GetTeam(TeamRequest request)
        {
            var entity = BasicLayer.GetTeam(request.TeamId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a team on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateTeam")]
        [ApiAuthorizationFilter(Permissions.ManageTeams, Permissions.CreateTeams)]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public async Task<IActionResult> CreateTeam(TeamCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Team
            {
                Name = request.Name
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateTeam(model, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);


            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing team
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateTeam")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        [ApiAuthorizationFilter(new[] { Permissions.EditTeam, Permissions.ManageTeams })]
        public async Task<IActionResult> UpdateTeam([EntityId] TeamUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeam(request.TeamId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;

            //Salvataggio
            var validations = await BasicLayer.UpdateTeam(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing team on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteTeam")]
        [ApiAuthorizationFilter(new[] { Permissions.DeleteTeam , Permissions.ManageTeams })]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public async Task<IActionResult> DeleteTeam([EntityId] TeamRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeam(request.TeamId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteTeam(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
