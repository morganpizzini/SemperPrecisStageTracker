using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.Contracts.Utilities;
using SemperPrecisStageTracker.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.API.Helpers;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for notification
    /// </summary>
    public class NotificationController : ApiControllerBase
    {
    [HttpPost]
    [Route("CreateNotificationSubscription")]
    [ProducesResponseType(typeof(object), 200)]
    public Task<IActionResult> CreateNotificationSubscription(NotificationSubscriptionCreateRequest request)
    {
      var currentUserId = PlatformUtils.GetIdentityUserId(User);
      var existingUser = AuthorizationLayer.GetUserById(currentUserId);
      if (existingUser == null)
            return Task.FromResult<IActionResult>(NotFound());

        var model = new NotificationSubscription
        {
            UserId = existingUser.Id,
            Url = request.Url,
            P256dh = request.P256dh,
            Auth = request.Auth
        };
        //Invocazione del service layer
        var validations = BasicLayer.CreateNotificationSubscription(model);

        if (validations.Count > 0)
            return BadRequestTask(validations);

        //Return contract
        return Reply(new OkResponse{Status = true});
    }
    [HttpPost]
        [Route("SendNotificationAsync")]
        [ProducesResponseType(typeof(object), 200)]
        public async  Task<IActionResult> SendNotificationAsync(SendNotificationSubscriptionRequest subscription)
        {
            // For a real application, generate your own
            var publicKey = "BL78AGXB1iRsP9CLGbzIIm5KNZvEgE36jbkImp0ow6U7Xp6cYji1C5-KGbPOxBTOX0fvABbNmfO9naQsTc79JzU";
            var privateKey = "9dtUPMWzZOklPblQVO6zbpyUz_CXsxzxfTWnlH4GfL8";
            return Ok(new OkResponse{Status = true});
            // var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            // var vapidDetails = new VapidDetails("mailto:morgan.pizzini@hotmail.com", publicKey, privateKey);
            // var webPushClient = new WebPushClient();
            // try
            // {
            //     var payload = JsonSerializer.Serialize(new
            //     {
            //         message= "ciao",
            //         url = $"myorders/blahblah",
            //     });
            //     await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            //     return Ok(true);
            // }
            // catch (Exception ex)
            // {
            //     Console.Error.WriteLine("Error sending push notification: " + ex.Message);
            //     return Ok(ex);

            // }
        }
    }
}
