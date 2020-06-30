using System;
using System.Dynamic;
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
        public CookieContainer CookieJar;

        private static WebClient APIClient = new WebClient();

        public Client(params string[] _CookieJar)
        {
            if (_CookieJar.Length == 0) CookieJar = null;
            else if (_CookieJar.Length == 1)
            {
                APIClient.Headers.Add
                    (
                        HttpRequestHeader.Cookie, $".ROBLOSECURITY={_CookieJar[0]};"
                    );

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

        public async Task<bool> Authorize()
        {
            Uri URI = new Uri("https://www.roblox.com/favorite/toggle");

            try
            {
                var Response = await APIClient.UploadStringTaskAsync(URI, "POST", ""); // Should error
            }
            catch (WebException e) when (e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Forbidden)
            {
                WebHeaderCollection ResponseHeaders = APIClient.ResponseHeaders;
                var Response = (HttpWebResponse)e.Response;
                Console.WriteLine(Response.Headers["X-CSRF-TOKEN"]);
                Console.WriteLine("403");
                APIClient.Headers["X-CSRF-TOKEN"] = Response.Headers["X-CSRF-TOKEN"];
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

        public async Task<bool> Promote(int UserID, int GroupID)
        {
            Uri URI = new Uri("https://groups.roblox.com/v1/groups/4953490/users/" + UserID);

            APIClient.Headers.Add(HttpRequestHeader.Accept, "application/json");

            APIClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            APIClient.Headers.Add(HttpRequestHeader.ContentEncoding, "application/json");

            for (int i = 0; i < APIClient.Headers.Count; i++)
            {
                Console.WriteLine(APIClient.Headers.GetKey(i));
            }

            var JSONData = new { roleId = 11 };

            try 
            { 
            var Response = await APIClient.UploadStringTaskAsync(URI, "PATCH", JsonConvert.SerializeObject(JSONData));
            } catch(WebException e)
            {
                Console.WriteLine(e.Response.Headers["Content-Type"]);
                var Response = (HttpWebResponse)e.Response;
                var encoding = System.Text.Encoding.UTF8;
                using (var reader = new System.IO.StreamReader(Response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                }
            }

            return true;
        }
    }
}
