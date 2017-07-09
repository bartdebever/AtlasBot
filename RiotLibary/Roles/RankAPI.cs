using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using Keys;
using RiotSharp.MatchEndpoint.Enums;
using RiotSharp.StaticDataEndpoint;
using RiotSharp.StatsEndpoint.Enums;
using RiotSharp.SummonerEndpoint;
using Season = RiotSharp.StatsEndpoint.Season;

namespace RiotLibary.Roles
{
    public class RankAPI
    {
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey, 500, 30000);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);

        public string GetRankingHarder(Summoner summoner, Queue queue)
        {
            string result = null;
            foreach (var stat in summoner.GetLeagues())
            {
                if (stat.Queue == queue)
                {
                    string rank = stat.Tier.ToString();
                    string division = stat.Entries.Where(y => y.PlayerOrTeamId == Convert.ToString(summoner.Id)).Select(y => y.Division).Single();
                    string wr = "";
                    try
                    {
                        var stats = summoner.GetStatsSummaries(Season.Season2017);
                        wr = stats.Where(y => y.PlayerStatSummaryType.ToString() == stat.Queue.ToString()).Select(
                            y =>
                            {
                                decimal wins = y.Wins;
                                decimal losses = y.Losses;
                                decimal total = wins + losses;
                                int winrate = Convert.ToInt32(wins / total * 100);
                                return winrate + "%";
                            }).First();
                    }
                    catch { }

                    result = rank + " " + division + " " + wr;
                }
            }
            return result;
        }

        public string GetAllRankings()
        {
            var summoner = api.GetSummoner(Region.euw, "BortTheBeaver");
            string result = "";
            var stats = summoner.GetStatsSummaries(Season.Season2017);
            foreach (var stat in stats)
            {
                if (stat.PlayerStatSummaryType == PlayerStatsSummaryType.RankedSolo5x5 || stat.PlayerStatSummaryType == PlayerStatsSummaryType.RankedFlexSR)
                {
                    decimal wins = stat.Wins;
                    decimal losses = stat.Losses;
                    decimal total = wins + losses;
                    int winrate = Convert.ToInt32(wins / total * 100);
                    result += stat.PlayerStatSummaryType + " " + stat.Wins + " " + stat.Losses + " " + winrate + "%";
                }
            }
            return result;
        }
        public string GetRankingSimple(Summoner summoner, Queue queue)
        {
            string result = "";
            foreach(var stat in summoner.GetLeagues())
            {
                if (stat.Queue == queue)
                {
                    result = stat.Tier.ToString();
                }
            }
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
