using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shootergroup
    /// </summary>
    public class ShooterGroupController : ApiControllerBase
    {
        /// <summary>
        /// Creates a shootergroup on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterGroup")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public IActionResult UpsertShooterGroup(ShooterGroupRequest request)
        {
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShootersInGroup(request.GroupId, request.ShooterIds);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{ Status= true});
        }

    }
}
