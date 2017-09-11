using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotNet;
using RiotNet.Models;

namespace RiotLibary.Roles
{
    public static class APIManager
    {
        private static IRiotClient client;
        public static IRiotClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new RiotClient(new RiotClientSettings
                    {
                        ApiKey = Keys.Keys.riotKey
                    }, PlatformId.EUW1);
                }
                return client;
            }
        }
    }
}
