using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers.V2
{
    public partial class PlacesController
    {
        [HttpGet("{id}/reservations")]
        [ProducesResponseType(typeof(IList<ReservationContract>), 200)]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        public Task<IActionResult> FetchPlaceSchedule(PlaceScheduleRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllReservationsByPlace(request.Id,request.FromDate,request.ToDate);
            var total = entities.Count;


            //Ritorno i contratti
            return Reply(
                new BaseResponse<IList<ReservationContract>>(
                    entities.As(x=>ContractUtils.GenerateContract(x))
                ));
        }
    }
}