using YoutubeWebhook.Interfaces;
using YoutubeWebhook.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeWebhook.Notifications
{
    public class TelegramNotification : INotificationTelegram
    {
        readonly HttpClient _httpClient;
        readonly TelegramModel _telegramModel;
        public TelegramNotification(HttpClient httpClient) 
        {
            _httpClient = httpClient;
            _telegramModel = new TelegramModel();
        }
        public async Task NotifyChannelAsync(YoutubeNotification notification, string urlToNofity)
        {
            _telegramModel.Title = notification.Title;
            _telegramModel.Link = notification.Link;
            _telegramModel.Resource = notification.Author;

            string json = JsonConvert.SerializeObject(_telegramModel);

            Console.WriteLine("Json for sending in telegram channel:");
            Console.WriteLine(json);
            HttpResponseMessage responseNotify = await _httpClient.PostAsync($"{urlToNofity}/sendPost", new StringContent(json, Encoding.UTF8, "application/json"));
            if (!responseNotify.IsSuccessStatusCode)
            {
                Console.WriteLine($"An error was thrown when sending the notification. Url Channel: {urlToNofity}, Exception: {await responseNotify.Content.ReadAsStringAsync()}");
            }
        } 
    }
}
