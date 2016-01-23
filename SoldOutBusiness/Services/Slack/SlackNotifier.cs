using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace SoldOutBusiness.Services.Slack
{
    public class SlackNotifier : ISlackNotifier
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackNotifier(string uri)
        {
            _uri = new Uri(uri);
        }

        //Post a message using a Payload object
        private void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data["payload"] = payloadJson;

                var response = client.UploadValues(_uri, "POST", data);

                //The response text is usually "ok"
                string responseText = _encoding.GetString(response);
            }
        }

        public void PostMessage(string message)
        {
            Payload payload = new Payload()
            {
#if DEBUG
                Username = "TestGibbon",
                IconEmoji = ":monkey:",
#else
                Username = "SearchMonkey",
                IconEmoji = ":monkey_face:",
#endif
                Text = message
            };

            PostMessage(payload);
        }
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks
    public class Payload
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon_emoji")]
        public string IconEmoji { get; set; }
    }
}
