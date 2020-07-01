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
            Client RClient = new Client("");
            await Task.Delay(TimeSpan.FromSeconds(3));
            RClient.Authorize();

            await RClient.SetRank(1085713226, 4953490, 33096185);

            // await RClient.GetSelf();
        }
    }
}
