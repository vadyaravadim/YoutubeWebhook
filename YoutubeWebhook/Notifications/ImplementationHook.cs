using YoutubeWebhook.Interfaces;
using YoutubeWebhook.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace YoutubeWebhook.Notifications
{
    public class ImplementationSubscribe : IWebHookYoutube
    {
        readonly HttpClient _httpClient;
        readonly YoutubeRequestData _dataYoutube;
        public ImplementationSubscribe(HttpClient httpClient, YoutubeRequestData data)
        {
            _httpClient = httpClient;
            _dataYoutube = data;
        }
        public async Task<HttpResponseMessage> NotifyYoutube(string channelId, HubMode hubMode)
        {
            _dataYoutube.ClientId = channelId;
            _dataYoutube.HubMode = hubMode;

            HttpResponseMessage response = await _httpClient.PostAsync(_dataYoutube.SubscribeUrl, new StringContent(_dataYoutube.GenerateData, Encoding.UTF8, "application/x-www-form-urlencoded"));

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                return response;
            }
            else
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Didn't receive any response from the hub"
                };
            }
        }

        public YoutubeNotification NotificationChannelCallback(HttpRequest request)
        {
            YoutubeNotification notification = null;
            try
            {
                notification = ConvertAtomToSyndication(request.Body);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"The video was deleted. Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Not supported message: {ex.Message}");
            }

            return notification;
        }

        public YoutubeNotification ConvertAtomToSyndication(Stream stream)
        {
            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                SyndicationItem item = feed.Items.FirstOrDefault();
                return new YoutubeNotification()
                {
                    ChannelId = GetElementExtensionValueByOuterName(item, "channelId"),
                    VideoId = GetElementExtensionValueByOuterName(item, "videoId"),
                    Title = item.Title.Text,
                    Published = item.PublishDate.ToString("dd/MM/yyyy"),
                    Updated = item.LastUpdatedTime.ToString("dd/MM/yyyy")
                };
            }
        }

        public string GetElementExtensionValueByOuterName(SyndicationItem item, string outerName)
        {
            if (item.ElementExtensions.All(x => x.OuterName != outerName)) return null;
            return item.ElementExtensions.Single(x => x.OuterName == outerName).GetObject<XElement>().Value;
        }
    }
}
