using YoutubeWebhook.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace YoutubeWebhook.Interfaces
{
    public interface IWebHookYoutube
    {
        Task<HttpResponseMessage> NotifyYoutube(string channelId, HubMode hubMode);
        YoutubeNotification NotificationChannelCallback(HttpRequest request);
        YoutubeNotification ConvertAtomToSyndication(Stream stream);
        string GetElementExtensionValueByOuterName(SyndicationItem item, string outerName);
    }
}
