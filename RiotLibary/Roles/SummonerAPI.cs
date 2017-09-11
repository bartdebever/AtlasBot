using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Keys;
using RiotLibary.Exceptions;
using RiotSharp;
using RiotSharp.LeagueEndpoint;
using RiotSharp.SummonerEndpoint;

namespace RiotLibary.Roles
{
    public class SummonerAPI
    {
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey,500,30000);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
        public Summoner GetSummoner(long id, Region region)
        {
            try
            {
                return api.GetSummoner(region, id);
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }

        public Summoner GetSummoner(string name, Region region)
        {
            try
            {
                return api.GetSummoner(region, name);
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }
        public long GetSummonerId(string name, Region region)
        {
            try
            {
                return api.GetSummoner(region, name).Id;
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }
        public List<MasteryPage> GetSummonerMasteryPages(string summonerName, Region region)
        {
            try
            {
                return api.GetSummoner(region, summonerName).GetMasteryPages();
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }

        public List<RunePage> GetRunePages(string summonerName, Region region)
        {
            try
            {
                return api.GetSummoner(region, summonerName).GetRunePages();
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }
    }
}
