using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace RoSharper
{
    class APIRequestBuilder
    {
        public static WebRequest CreateWebRequest(bool authorized, string URI, string CookieJar="", string RequestMethod="GET", string PostData = "", string Token="")
        {
            WebRequest APIRequest = WebRequest.Create(new Uri(URI));
            APIRequest.Method = RequestMethod;
            if (authorized)
            {
                // Add relevant headers
                APIRequest.Method = RequestMethod;
                APIRequest.Headers.Add("Cookie", $".ROBLOSECURITY={CookieJar};");
                APIRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:66.0) Gecko/20100101 Firefox/66.0");
                APIRequest.Headers.Add("Accept", "application/json, text/plain, */*");
                APIRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                APIRequest.Headers.Add("Origin", "https://www.roblox.com");
                APIRequest.Headers.Add("Content-Type", "application/json");
                APIRequest.Headers.Add("DNT", "1");
                APIRequest.Headers.Add("x-csrf-token", Token);
                APIRequest.Headers.Add("Access-Control-Expose-Headers", "X-CSRF-TOKEN");
                APIRequest.ContentType = "application/json; charset=utf-8";
            }

            if (PostData != "")
            {
                using(var sw = new StreamWriter(APIRequest.GetRequestStream()))
                {
                    sw.Write(PostData);
                    sw.Flush();
                    sw.Close();
                }
            }

            return APIRequest;
        }
    }
}
