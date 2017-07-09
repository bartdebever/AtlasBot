using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp.ChampionEndpoint;
using RiotSharp.SummonerEndpoint;
using Keys;
using RiotLibary.Exceptions;
using RiotLibary.Extras;
using RiotSharp;

namespace RiotLibary.Roles
{
    public class ChampionAPI
    {
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey, 500, 30000);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
        public List<MainChampion> Get5MainChampions(Summoner summoner)
        {
            List<MainChampion> result = new List<MainChampion>();
            List<MainChampion> champs = new List<MainChampion>();
            (api.GetMatchList(summoner.Region, summoner.Id, null, null, null, new DateTime(2016, 12, 7))).Matches.ForEach(
                match =>
                {
                    bool found = false;
                    if (champs.Count != 0)
                    {
                        foreach (var champ in champs)
                        {
                            if (champ.Id == match.ChampionID)
                            {
                                champ.IncreaseCount();
                                found = true;
                            }
                        }
                    }
                    if (found == false)
                    {
                        champs.Add(new MainChampion(match.ChampionID));
                    }
                });
            int ChampCount = 5;
            if (champs.Count < 5)
            {
                ChampCount = champs.Count;
            }
            champs.Sort();
            for (int i = 0; i < ChampCount; i++)
            {
                champs[i].Name = sApi.GetChampion(Region.eune, Convert.ToInt32(champs[i].Id)).Name;
                result.Add(champs[i]);
            }
            return result;
        }

        public Champion GetChampion(int id, Region region)
        {
            return api.GetChampion(region, id);
        }

        public int GetChampionId(string name)
        {
            foreach (var champ in sApi.GetChampions(Region.euw).Champions)
            {
                if (champ.Value.Name.ToLower() == name.ToLower())
                {
                    return champ.Value.Id;
                }
            }
            throw new ChampionNotFoundException();
        }

        public Champion GetChampion(string name)
        {
            return api.GetChampion(Region.euw, GetChampionId(name));
        }

        public string GetChampionName(int id)
        {
            return sApi.GetChampion(Region.eune, id).Name;
        }
    }
}
