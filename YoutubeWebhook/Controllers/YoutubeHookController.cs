using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using YoutubeWebhook.Interfaces;
using YoutubeWebhook.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace YoutubeWebhook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeHookController : ControllerBase
    {
        readonly IWebHookYoutube _hookYoutube;
        readonly INotificationTelegram _telegram;
        public YoutubeHookController(IWebHookYoutube hookYoutube, INotificationTelegram telegram)
        {
            _hookYoutube = hookYoutube;
            _telegram = telegram;
        }

        [HttpGet]
        [Route("subscribe")]
        public async Task<HttpResponseMessage> Subscribe(string channelId)
        {
            try
            {
                return await _hookYoutube.NotifyYoutube(channelId, HubMode.subscribe);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent($"Error unsubscribe Channel ID : {channelId}, {ex.Message}")
                };
            }
        }

        [HttpGet]
        [Route("unsubscribe")]
        public async Task<HttpResponseMessage> Unsubscribe(string channelId)
        {
            try
            {
                return await _hookYoutube.NotifyYoutube(channelId, HubMode.unsubscribe);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent($"Error unsubscribe Channel ID : {channelId}, {ex.Message}")
                };
            }
        }

        [HttpGet]
        [HttpPost]
        public async Task<string> NotificationCallbackAsync()
        {
            Console.WriteLine("Notification accepted");
            string challenge = Request.Query["hub.challenge"];

            if (string.IsNullOrEmpty(challenge))
            {
                challenge = "OK";

                YoutubeNotification notification = _hookYoutube.NotificationChannelCallback(Request);

                if (notification == null || !notification.IsNewVideo)
                {
                    return challenge;
                }
            }

            return challenge;
        }

    }
}
