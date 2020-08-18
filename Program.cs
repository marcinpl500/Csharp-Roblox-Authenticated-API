using System;
using System.Collections.Generic;
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

            // await RClient.SetRankTest(4953490, 1505886708, 34179654);

            List<Role> Roles = await RClient.GetGroupRanks(499);

            foreach(Role role in Roles)
            {
                Console.WriteLine($"{role.Rank} | {role.Name} | {role.MemberCount}");
            }
        }
    }
}
