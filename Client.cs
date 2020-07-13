using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoSharper
{
    class Client
    {
        public string CookieJar = "";
        public string Token = "";
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

        public async Task<User> GetSelf()
        {
            Uri URI = new Uri("https://www.roblox.com/my/profile");

            var Response = await APIClient.DownloadStringTaskAsync(URI);

            User UserData = JsonConvert.DeserializeObject<User>(Response);
            Console.WriteLine(UserData.Username);
            return UserData;
        }

        public async Task<bool> SetRankTest(int GroupID, int UserID, int RankID)
        {
            string PostData = $"{{\"roleId\": \"{RankID}\"}}";
            var Request = APIRequestBuilder.CreateWebRequest(true, $"https://groups.roblox.com/v1/groups/{GroupID}/users/{UserID}", CookieJar, "PATCH", PostData);
            try
            {
                WebResponse Response = await Request.GetResponseAsync();
            } catch (WebException e)
            {
                string token = e.Response.Headers["x-csrf-token"];
                Request = APIRequestBuilder.CreateWebRequest(true, $"https://groups.roblox.com/v1/groups/{GroupID}/users/{UserID}", CookieJar, "PATCH", PostData, token);
                WebResponse Response = await Request.GetResponseAsync();
                using (var reader = new StreamReader(Response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
            return true;
        }

        public async Task<bool> GetGroupRanks(int GroupID)
        {
            var Request = APIRequestBuilder.CreateWebRequest(false, $"https://groups.roblox.com/v1/groups/{GroupID}/roles");
            string JSON = "";
            try
            {
                WebResponse Response = await Request.GetResponseAsync();
                using(var Reader = new StreamReader(Response.GetResponseStream()))
                {
                    JSON = Reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            var Data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JSON);
            var Roles = Data["roles"];
            Console.WriteLine(Roles[1]);
            return true;
        }
    }
}
