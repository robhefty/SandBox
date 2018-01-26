using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace SandBox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // You need to set your own keys and screen name
            string oAuthConsumerKey = "iGArkVBkkvVgVgqTtgjj356HA";
            var oAuthConsumerSecret = "mcKrdyq3benfSOPOBBa5htBncDjdPKtoQo9ZOsHnAwj9Xx99sG";
            var oAuthUrl = "https://api.twitter.com/oauth2/token";
            var screenname = "robhefty";

            // Do the Authenticate
            var authHeaderFormat = "Basic {0}";

            var authHeader = string.Format(authHeaderFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(oAuthConsumerKey) + ":" +
                Uri.EscapeDataString((oAuthConsumerSecret)))
            ));

            var postBody = "grant_type=client_credentials";

            System.Net.HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            authRequest.Headers.Add("Authorization", authHeader);
            authRequest.Method = "POST";
            authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (Stream stream = authRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            authRequest.Headers.Add("Accept-Encoding", "gzip");

            WebResponse authResponse = authRequest.GetResponse();
            // deserialize into an object
            TwitAuthenticateResponse twitAuthResponse;
            using (authResponse)
            {
                using (var reader = new StreamReader(authResponse.GetResponseStream()))
                {
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    var objectText = reader.ReadToEnd();
                    twitAuthResponse = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);
                }
            }

            //// Do the timeline
            //var timelineFormat = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts=1&exclude_replies=1&count=5";
            //var timelineUrl = string.Format(timelineFormat, screenname);
            //HttpWebRequest timeLineRequest = (HttpWebRequest)WebRequest.Create(timelineUrl);
            //var timelineHeaderFormat = "{0} {1}";
            //timeLineRequest.Headers.Add("Authorization", string.Format(timelineHeaderFormat, twitAuthResponse.token_type, twitAuthResponse.access_token));
            //timeLineRequest.Method = "Get";
            //WebResponse timeLineResponse = timeLineRequest.GetResponse();
            //var timeLineJson = string.Empty;
            //using (timeLineResponse)
            //{
            //    using (var reader = new StreamReader(timeLineResponse.GetResponseStream()))
            //    {
            //        timeLineJson = reader.ReadToEnd();
            //    }
            //}

            // Do the search
            var searchFormat = "https://api.twitter.com/1.1/search/tweets.json?q={0}&count=10&result_type=popular";
            var searchURL = string.Format(searchFormat, "trainspotting");
            HttpWebRequest searchRequest = (HttpWebRequest)WebRequest.Create(searchURL);
            var timelineHeaderFormat = "{0} {1}";
            searchRequest.Headers.Add("Authorization", string.Format(timelineHeaderFormat, twitAuthResponse.token_type, twitAuthResponse.access_token));
            searchRequest.Method = "Get";
            WebResponse searchResponse = searchRequest.GetResponse();
            var searchJson = string.Empty;
            using (searchResponse)
            {
                using (var reader = new StreamReader(searchResponse.GetResponseStream()))
                {
                    searchJson = reader.ReadToEnd();
                }
            }

        }


        public class TwitAuthenticateResponse
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
        }


    }
}
