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
        [HttpGet("{id}/schedules")]
        [ProducesResponseType(typeof(IList<ScheduleContract>), 200)]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.CreatePlaces)]
        public Task<IActionResult> FetchSchedules(TakeSkipBaseRequestId request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllSchedulesByPlace(request.Id).AsQueryable();
            var total = entities.Count();

            if (request.Skip.HasValue)
                entities = entities.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                entities = entities.Take(request.Take.Value);

            //Ritorno i contratti
            return Reply(
                new BaseResponse<IList<ScheduleContract>>(
                    entities.As(ContractUtils.GenerateContract),
                    total,
                    request.Take.HasValue ?
                        Url.Action(action: nameof(FetchSchedules), controller: "Places", new { take = request.Take, refId = request.Id, skip = request.Take + (request?.Skip ?? 0) }) :
                        string.Empty
                ));
        }
    }
}