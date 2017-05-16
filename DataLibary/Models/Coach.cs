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
        public Role Role { get; private set; }
        public Champion Champion { get; private set; }
        public Region Region { get; private set; }

        public Coach(ulong coachid, Role role, Champion champion, Region region)
        {
            this.CoachId = coachid;
            this.Role = role;
            this.Champion = champion;
            this.Region = region;
        }
    }
}
