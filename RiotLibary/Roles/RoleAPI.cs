using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using RiotLibary.Exceptions;
using RiotSharp;
using RiotSharp.SummonerEndpoint;

namespace RiotLibary.Roles
{
    public class RoleAPI
    {
        private RiotApi api = RiotApi.GetInstance(Keys.Keys.riotKey);
        private StaticRiotApi sApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
        public string GetRole(Summoner summoner)
        { 
            int[] roles = new int[6] {0,0,0,0,0,0};
            try
            {
                api.GetMatchList(summoner.Region, summoner.Id, null, null, null, new DateTime(2016, 12, 7)).Matches.ForEach(match =>
                {
                    if (match.Queue == Queue.RankedSolo5x5 || match.Season.ToString().Contains("Pre"))
                    {
                        if (match.Lane == Lane.Top)
                        {
                            roles[0] += 1;
                        }
                        else if (match.Lane == Lane.Jungle)
                        {
                            roles[1] += 1;
                        }
                        else if (match.Lane == Lane.Mid || match.Lane == Lane.Middle)
                        {
                            roles[2] += 1;
                        }
                        else if (match.Lane == Lane.Bot || match.Lane == Lane.Bottom && match.Role == RiotSharp.Role.DuoCarry)
                        {
                            roles[3] += 1;
                        }
                        else if (match.Lane == Lane.Bot || match.Lane == Lane.Bottom && match.Role == RiotSharp.Role.DuoSupport)
                        {
                            roles[4] += 1;
                        }
                        else if (match.Role == RiotSharp.Role.None)
                        {
                            roles[5] += 1;
                        }
                    }
                });
                if (roles.ToList().IndexOf(roles.Max()) == 0)
                {
                    return "Top";
                }
                else if (roles.ToList().IndexOf(roles.Max()) == 1)
                {
                    return "Jungle";
                }
                else if (roles.ToList().IndexOf(roles.Max()) == 2)
                {
                    return "Mid";
                }
                else if (roles.ToList().IndexOf(roles.Max()) == 3)
                {
                    return "ADC";
                }
                else if (roles.ToList().IndexOf(roles.Max()) == 4)
                {
                    return "Support";
                }
                else if (roles.ToList().IndexOf(roles.Max()) == 5)
                {
                    return "Fill";
                }
            }
            catch { }
            
            throw new NoRankedGamesException("No roles found");
        }
    }
}
