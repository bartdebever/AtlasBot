using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using Keys;
using RiotSharp.StaticDataEndpoint;
using RiotSharp.SummonerEndpoint;

namespace RiotLibary.Roles
{
    public class RankAPI
    {
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);

        public string GetRankingHarder(Summoner summoner, Queue queue)
        {
            string result = null;
            summoner.GetLeagues().ForEach(stat =>
            {
                if (stat.Queue == queue)
                {
                    string rank = stat.Tier.ToString();
                    string division = stat.Entries.Where(y => y.PlayerOrTeamId == summoner.Id.ToString()).Select(y => y.Division).Single();
                    result =  (rank + " " + division);
                }
            });
            return result;
        }

        public string GetRankingSimple(Summoner summoner, Queue queue)
        {
            string result = "";
            summoner.GetLeagues().ForEach(stat =>
            {
                if (stat.Queue == queue)
                {
                    result = stat.Tier.ToString();
                }
            });
            return result;
        }

        public int GetLp(Summoner summoner, Queue queue)
        {
            int result = 0;
            summoner.GetLeagues().ForEach(stat =>
            {
                if (stat.Queue == queue)
                {
                    result =
                        stat.Entries.Where(y => y.PlayerOrTeamId == summoner.Id.ToString())
                            .Select(y => y.LeaguePoints)
                            .Single();
                }
            });
            return result;
        }
    }
}
