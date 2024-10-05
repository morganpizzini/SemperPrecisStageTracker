using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTracker.Contracts.Requests;
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


            var bays = BasicLayer.FetchAllBays(request.Id);
            var users = BasicLayer.FetchUsersByIds(entities.Select(x => x.UserId).ToList());

            //Ritorno i contratti
            return Reply(
                new BaseResponse<IList<ReservationContract>>(
                    entities.As(x=>ContractUtils.GenerateContract(x,bays.FirstOrDefault(b=>b.Id == x.BayId),users.FirstOrDefault(u=>u.Id == x.UserId)))
                ));
        }
    }
}