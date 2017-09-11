using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotLibraryV2.Tools;
using RiotNet.Models;

namespace RiotLibraryV2.API
{
    public class SummonerAPI
    {
        public static async Task<Summoner> GetSummoner(string summonername, string region)
        {
            return await APIManager.Client.GetSummonerBySummonerNameAsync("BortTheBeaver", PlatformConverter.GetPlatform(region));
        }

        public static async Task<Summoner> GetSummoner(int id, string region)
        {
            return await APIManager.Client.GetSummonerByAccountIdAsync(id, PlatformConverter.GetPlatform(region));
        }
    }
}
