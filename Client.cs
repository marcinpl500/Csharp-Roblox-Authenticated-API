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
                APIRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:66.0) Gecko/20100101 Firefox/66.0");
                APIRequest.Headers.Add("Accept", "application/json, text/plain, */*");
                APIRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                APIRequest.Headers.Add("Origin", "https://www.roblox.com");
                APIRequest.Headers.Add("DNT", "1");
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

        public async Task<bool> SetRank(int GroupID, int UserID, int RankID)
        {
            Uri URI = new Uri($"https://groups.roblox.com/v1/groups/{GroupID}/users/{UserID}");

            WebRequest APIRequest = WebRequest.Create(URI);
            APIRequest.Method = "PATCH";
            string CookieFormatted = ".ROBLOSECURITY=" + CookieJar + ";";
            Console.WriteLine(CookieJar);
            APIRequest.Headers.Add("Cookie", CookieJar);
            APIRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:66.0) Gecko/20100101 Firefox/66.0");
            APIRequest.Headers.Add("Accept", "application/json, text/plain, */*");
            APIRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            APIRequest.Headers.Add("Origin", "https://www.roblox.com");
            APIRequest.Headers.Add("Content-Type", "application/json");
            APIRequest.Headers.Add("DNT", "1");
            APIRequest.Headers.Add("x-csrf-token", Token);

<<<<<<< HEAD
=======
            APIRequest.Headers.Add(HttpRequestHeader.Cookie, $".ROBLOSECURITY={CookieJar};");
            APIRequest.Headers.Add("x-csrf-token", Token);
            APIRequest.Headers.Add("Access-Control-Expose-Headers", "X-CSRF-TOKEN");

            APIRequest.ContentType = "application/json; charset=utf-8";
>>>>>>> e5d055fcb920f1371a5d5c9e861da0ca4cfc0f81

            Console.WriteLine("\n\nMy headers");
            foreach (var key in APIRequest.Headers.Keys)
            {
                Console.WriteLine($"{key} || {APIRequest.Headers[key.ToString()]}");
            }
            string PostData = $"{{\"roleId\": \"{RankID}\"}}";
            Console.WriteLine(PostData);
            Console.WriteLine(PostData);
            using (StreamWriter StreamW = new StreamWriter(APIRequest.GetRequestStream()))
            {
                StreamW.Write(PostData);
                StreamW.Flush();
                StreamW.Close();

                WebResponse HTTPResponse;
                try
                {
                    HTTPResponse = await APIRequest.GetResponseAsync();
                    HTTPResponse = (HttpWebResponse)HTTPResponse;

                    using (StreamReader x = new StreamReader(HTTPResponse.GetResponseStream()))
                    {
                        var Result = x.ReadToEnd();
                        Console.WriteLine(x);
                    };
                }
                catch(WebException e)
                {
                    Console.WriteLine(e);
                    Token = e.Response.Headers["x-csrf-token"];
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    await SetRank(UserID, GroupID, RankID);
                }

            }

            return true;
        }
    }
}