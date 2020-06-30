using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace RoSharper
{
    class Program
    {
        public static WebClient APIClient = new WebClient();
        static async Task Main(string[] args)
        {
            Client RClient = new Client("COOKIE");
            await RClient.Authorize();

            await RClient.Promote(4953490, 4953490);

            await RClient.GetSelf();
        }
    }
}
