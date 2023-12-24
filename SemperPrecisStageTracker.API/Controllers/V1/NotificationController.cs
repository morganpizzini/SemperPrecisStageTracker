using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.Contracts.Utilities;
using SemperPrecisStageTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebPush;
using SemperPrecisStageTracker.Contracts.Requests;
using Asp.Versioning;
using SemperPrecisStageTracker.API.Helpers;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for notification
    /// </summary>
    [ApiVersion("1.0")]
    public class NotificationController : ApiControllerBase
    {
        private string _privateKey;
        private string _publicKey;
        private string _vapidUser;

        public NotificationController(IConfiguration configuration)
        {
            _publicKey = configuration["webPushPublic"];
            _privateKey = configuration["webPushPrivate"];
            _vapidUser = configuration["webPushUser"];
        }

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
            return Reply(new OkResponse { Status = true });
        }
        [HttpPost]
        [Route("SendNotificationAsync")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> SendNotificationAsync(SendNotificationSubscriptionRequest subscription)
        {
            await Task.CompletedTask;
            return Ok(new OkResponse { Status = true });
            // var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            // var vapidDetails = new VapidDetails($"mailto:{_vapidUser}", _publicKey, _privateKey);
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

        [HttpPost]
        [Route("CallShooter")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> CallShooter(CallShooterRequest request)
        {
            // For a real application, generate your own
            var currentUserId = PlatformUtils.GetIdentityUserId(User);
            var existingUser = AuthorizationLayer.GetUserById(currentUserId);

            if (existingUser == null)
                return NotFound();

            var subscriptions = BasicLayer.FetchNotificationSubscriptionsByUserId(request.ShooterId);

            var vapidDetails = new VapidDetails($"mailto:{_vapidUser}", _publicKey, _privateKey);


            var pushSubscriptions = subscriptions.Select(x => new PushSubscription(x.Url, x.P256dh, x.Auth)).ToList();
            var webPushClient = new WebPushClient();
            try
            {
                var message = string.Empty;
                switch ((int)request.Context)
                {
                    case (int)CallShooterContextEnum.MatchDirector:
                        var userStage = BasicLayer.GetSOStage(request.MatchId, existingUser.Id);
                        message = $"{existingUser.FirstName} {existingUser.LastName} ti sta cercando allo stage {userStage.Index}:{userStage.Name}!";
                        break;
                    default:
                        message = $"{existingUser.FirstName} {existingUser.LastName} ti sta cercando!";
                        break;
                }
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = string.Empty,
                });
                if (pushSubscriptions.Count == 0)
                {
                    return Ok(new OkResponse { Status = false, Errors = new List<string> { "NoSubscriptions" } });
                }
                var tasks = pushSubscriptions.Select(pushSubscription => webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails)).ToList();
                await Task.WhenAll(tasks);
                return Ok(new OkResponse() { Status = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                return Ok(ex);

            }
        }
    }
}
