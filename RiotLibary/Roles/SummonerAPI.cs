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
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
        public Summoner GetSummoner(int id, Region region)
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

        public string GetSummonerName(int id, Region region)
        {
            try
            {
                return api.GetSummoner(region, id).Name;
            }
            catch
            {
                throw  new SummonerNotFoundException();
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

        public long GetSummonerLevel(string name, Region region)
        {
            try
            {
                return api.GetSummoner(region, name).Level;
            }
            catch
            {
                throw  new SummonerNotFoundException();
            }
        }

        public long GetSummonerLevel(int id, Region region)
        {
            try
            {
                return api.GetSummoner(region, id).Level;
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }

        public List<MasteryPage> GetSummonerMasteryPages(int id, Region region)
        {
            try
            {
                return api.GetSummoner(region, id).GetMasteryPages();
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

        public List<League> GetLeagues(int id, Region region)
        {
            try
            {
                return api.GetSummoner(region, id).GetLeagues();
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }
        public List<League> GetLeagues(string summonerName, Region region)
        {
            try
            {
                return api.GetSummoner(region, summonerName).GetLeagues();
            }
            catch
            {
                throw new SummonerNotFoundException();
            }
        }

        public long GetLevel(string summonerName, Region region)
        {
            return GetSummoner(summonerName, region).Level;
        }
        public long GetLevel(int id, Region region)
        {
            return GetSummoner(id, region).Level;
        }
    }
}
