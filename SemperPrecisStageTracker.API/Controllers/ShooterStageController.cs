using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Task<IActionResult> UpsertShooterStage(ShooterStageRequest request)
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
                Procedural = request.Procedural,
                Disqualified = request.Disqualified
            };
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShooterStage(shooterStage);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse{ Status= true});
        }

    }
}
