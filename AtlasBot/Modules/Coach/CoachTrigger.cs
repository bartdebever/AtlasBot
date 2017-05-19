using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using Discord.Commands;
using RiotLibary.Roles;
using RiotSharp.ChampionEndpoint;

namespace AtlasBot.Modules.Coach
{
    public class CoachTrigger
    {
        public void AddCoach(ulong discordid, string championname, string role)
        {
            ChampionAPI championApi = new ChampionAPI();
            int champid = championApi.GetChampionId(championname);
            bool found = false;
            foreach (string line in DataLibary.Models.Roles.NormalRoles())
            {
                if (line.ToLower() == role.ToLower())
                {
                    found = true;
                }
            }
            if (found == true)
            {
                new CoachRepo(new CoachContext()).AddCoach(new DataLibary.Models.Coach(discordid, role, champid));
            }
            else
            {
                throw new Exception("Role or Champion not found.");
            }
        }

        public void AddChampion(ulong discordid, string championname)
        {
            ChampionAPI championApi = new ChampionAPI();
            int champid = championApi.GetChampionId(championname);
            new CoachRepo(new CoachContext()).AddChampionToCoach(champid, discordid);
        }
    }
}
