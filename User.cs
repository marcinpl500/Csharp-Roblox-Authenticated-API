using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;


namespace RoSharper
{
    class User
    {
        public string Username { get; set; }
        public string Id { get; set; }
        public bool IsOnline { get; set; }
        public string AvatarURI { get; set; }
        // public string AvatarURI { get; private set; }
    }
}
