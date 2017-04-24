using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.ChampionEndpoint;
using RiotSharp.ChampionMasteryEndpoint;
using RiotSharp.SummonerEndpoint;
using ToolKit;

namespace RiotLibary.Roles
{
    public class MasteryAPI
    {
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
        public int GetLevel(Summoner summoner, Champion champion)
        {
            return api.GetChampionMastery(RiotSharpAddition.RegionToPlatform(summoner.Region), summoner.Id, Convert.ToInt32(champion.Id)).ChampionLevel;
        }
        //Simply returns the ammount of point a person has
        public int GetPoints(Summoner summoner, Champion champion)
        {
            return api.GetChampionMastery(RiotSharpAddition.RegionToPlatform(summoner.Region), summoner.Id, Convert.ToInt32(champion.Id)).ChampionPoints;
        }

        public List<ChampionMastery> GetChampionMasterys(Summoner summoner)
        {
            return api.GetChampionMasteries(RiotSharpAddition.RegionToPlatform(summoner.Region), summoner.Id);
        }
    }
}
