using System;
using System.Web;

namespace YoutubeWebhook.Model
{
    public enum HubMode
    {
        subscribe,
        unsubscribe
    }

    public class YoutubeRequestData
    {
        public HubMode HubMode { get; set; }
        public string ClientId { get; set; }
        public string CallbackUrl { get; set; }
        public string TopicUrl { get; set; }
        public string SubscribeUrl { get; set; }
        public string GenerateData { 
            get
            {
                if (string.IsNullOrEmpty(CallbackUrl) || string.IsNullOrEmpty(TopicUrl) || string.IsNullOrEmpty(ClientId))
                {
                    throw new Exception($"One or more parameters are empty! CallbackUrl: {CallbackUrl}, TopicUrl: {TopicUrl}, ClientId: {ClientId}");
                }

                return $"hub.mode={HubMode.ToString()}&hub.verify_token={Guid.NewGuid().ToString()}&hub.verify=async&hub.callback={HttpUtility.UrlEncode(CallbackUrl)}&hub.topic={HttpUtility.UrlEncode(TopicUrl+ClientId)}&hub.challenge={Guid.NewGuid()}";
            }
        }
    }

    public class YoutubeNotification
    {
        public string Author { get; set; }
        public string Id { get; set; }
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Link
        {
            get
            {
                return $"https://www.youtube.com/watch?v={VideoId}";
            }
        }
        public string Published { get; set; }
        public string Updated { get; set; }
        public bool IsNewVideo
        {
            get
            {
                return Published == Updated && !string.IsNullOrEmpty(Published);
            }
        }
    }
}
