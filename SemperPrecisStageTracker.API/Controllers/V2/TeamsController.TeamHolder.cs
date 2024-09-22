using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using System.ComponentModel.DataAnnotations;


namespace SemperPrecisStageTracker.API.Controllers.V2
{
    [ApiVersion("2.0")]
    public partial class TeamsController : ApiControllerBase
    {
        [HttpPost("{id}/team-holder")]
        [ApiAuthorizationFilter(Permissions.ManageTeams, Permissions.EditTeam)]
        public IActionResult CreateTeamHolder(EntityBaseRequestId<TeamHolderCreateV2Request> request)
        {
            var entity = BasicLayer.GetTeam(request.Id);

            if (entity == null)
                return NotFound();

            IList<ValidationResult> validations = new List<ValidationResult>();

            var teamOwners = BasicLayer.FetchTeamHolderUsersByTeam(request.Id);

            if (teamOwners.Any(x => x.Id == request.Body.UserId))
            {
                validations.Add(new ValidationResult("User already in team holder list"));
                return BadRequest(validations);
            }

            var model = new TeamHolder
            {
                Description = request.Body.Description,
                UserId = request.Body.UserId,
                TeamId = request.Id
            };

            validations = BasicLayer.UpsertTeamHolder(model);

            if (validations.Count > 0)
                return BadRequest(validations);

            return NoContent();
        }

        [HttpDelete("{id}/team-holder/{userId}")]
        [ApiAuthorizationFilter(Permissions.ManageTeams, Permissions.EditTeam)]
        [ProducesResponseType(204)]
        public IActionResult DeleteBaySchedule(TeamHolderV2DeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeamHolderByTeamAndShooterId(request.Id, request.UserId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var validations = BasicLayer.DeleteTeamHolder(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}