using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoSharper
{
    class Client
    {
        public string CookieJar = "";
        private string Token = " empty ";
        private static WebClient APIClient = new WebClient();

        public Client(params string[] _CookieJar)
        {
            if (_CookieJar.Length == 0) CookieJar = null;
            else if (_CookieJar.Length == 1)
            {
                CookieJar = _CookieJar[0];
                APIClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                APIClient.Encoding = System.Text.Encoding.UTF8;
            }
        }

        public async Task<User> GetUserByUsername(string Username)
        {
            Uri URI = new Uri("https://api.roblox.com/users/get-by-username?username=" + Username);

            var Response = await APIClient.DownloadStringTaskAsync(URI);

            User UserData = JsonConvert.DeserializeObject<User>(Response);

            UserData.AvatarURI = "https://www.roblox.com/headshot-thumbnail/image?userId=" + UserData.Id + "&width=420&height=420&format=png";

            return UserData;
        }

        public async Task<User> GetUserByUserID(int ID)
        {
            Uri URI = new Uri("https://api.roblox.com/users/" + ID);

            var Response = await APIClient.DownloadStringTaskAsync(URI);

            User UserData = JsonConvert.DeserializeObject<User>(Response);

            UserData.AvatarURI = "https://www.roblox.com/headshot-thumbnail/image?userId=" + ID + "&width=420&height=420&format=png";

            return UserData;
        }

        public bool Authorize()
        {
            Uri URI = new Uri("https://www.roblox.com/favorite/toggle");

            try
            {
                var APIRequest = WebRequest.Create(URI);
                APIRequest.Method = "POST";
                APIRequest.Headers.Add(HttpRequestHeader.Cookie, CookieJar);
                var Response = APIRequest.GetResponse();

            }
            catch (WebException e)
            {
                Console.WriteLine("Roblox Headers");
                foreach (var key in e.Response.Headers.Keys) Console.WriteLine($"{key} || {e.Response.Headers[key.ToString()]}");
                Token = e.Response.Headers["x-csrf-token"];
            };

            return true;
        }

        public async Task<User> GetSelf()
        {
            Uri URI = new Uri("https://www.roblox.com/my/profile");

            var Response = await APIClient.DownloadStringTaskAsync(URI);
            
            User UserData = JsonConvert.DeserializeObject<User>(Response);
            Console.WriteLine(UserData.Username);
            return UserData;
        }

        public async Task<bool> Shout(int UserID, int GroupID)
        {
            Uri URI = new Uri($"https://groups.roblox.com/v1/groups/${GroupID}/status");

            WebRequest APIRequest = WebRequest.Create(URI);
            APIRequest.Method = "PATCH";

            APIRequest.Headers.Add(HttpRequestHeader.Cookie, $".ROBLOSECURITY={CookieJar};");
            APIRequest.Headers.Add("x-csrf-token", Token);
            APIRequest.Headers.Add("Access-Control-Expose-Headers", "X-CSRF-TOKEN");

            APIRequest.ContentType = "application/json; charset=utf-8";

            Console.WriteLine("\n\nMy headers");
            foreach(var key in APIRequest.Headers.Keys)
            {
                Console.WriteLine($"{key} || {APIRequest.Headers[key.ToString()]}");
            }
            string PostData = "{\"message\": \"ok gamers\"}";
            Console.WriteLine(PostData);
            using(StreamWriter StreamW = new StreamWriter(APIRequest.GetRequestStream()))
            {
                StreamW.Write(PostData);
                StreamW.Flush();
                StreamW.Close();

                var HTTPResponse = await APIRequest.GetResponseAsync();

                HTTPResponse = (HttpWebResponse)HTTPResponse;

                using(StreamReader x = new StreamReader(HTTPResponse.GetResponseStream()))
                {
                    var Result = x.ReadToEnd();
                    Console.WriteLine(x);
                };
            }

            return true;
        }
    }
}
