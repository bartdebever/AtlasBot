using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.ChampionEndpoint;

namespace DataLibary.Models
{
    public class Coach
    {
        public ulong CoachId { get; private set; }
        public string Role { get; private set; }
        public int Champion { get; private set; }

        public Coach(ulong coachid, string role, int champion)
        {
            this.CoachId = coachid;
            this.Role = role;
            this.Champion = champion;
        }
    }
}
