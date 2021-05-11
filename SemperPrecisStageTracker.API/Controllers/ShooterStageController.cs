using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooterStage
    /// </summary>
    public class ShooterStageController : ApiControllerBase
    {
        /// <summary>
        /// Creates a shooterStage on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterStage")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public IActionResult UpsertShooterStage(ShooterStageRequest request)
        {
            var shooterStage = new ShooterStage{
                StageId = request.StageId,
                Time = request.Time,
                ShooterId = request.ShooterId,
                DownPoints = request.DownPoints,
                Procedurals = request.Procedurals,
                HitOnNonThreat = request.HitOnNonThreat,
                FlagrantPenalties = request.FlagrantPenalties,
                Ftdr = request.Ftdr,
                Disqualified = request.Disqualified
            };
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShooterStage(shooterStage);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{ Status= true});
        }

    }
}
