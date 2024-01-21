using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
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
    [ApiVersion("1.0")]
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
        public async Task<IActionResult> UpsertShooterStage([FromBody]ShooterStageRequest request)
        {
            var shooterStage = new UserStageString
            {
                StageStringId = request.StageStringId,
                StageId = request.StageId,
                Time = request.Time,
                UserId = request.ShooterId,
                DownPoints = request.DownPoints,
                Procedurals = request.Procedurals,
                Bonus = request.Bonus,
                HitOnNonThreat = request.HitOnNonThreat,
                FlagrantPenalties = request.FlagrantPenalties,
                Ftdr = request.Ftdr,
                Warning = request.Warning,
                Disqualified = request.Disqualified,
                Notes = request.Notes
            };
            //Invocazione del service layer
            var validations = await BasicLayer.UpsertShooterStage(shooterStage,PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse { Status = true });
        }
        /// <summary>
        /// Creates a shooterStage on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterStageString")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> DeleteShooterStageString([FromBody]DeleteShooterStageRequest request)
        {
            //Invocazione del service layer
            var validations = await BasicLayer.DeleteShooterStageString(request.ShooterId,request.StageId,request.StageStringId,PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse { Status = true });
        }
    }
}
