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
        public int Id { get; private set; }
        public ulong CoachId { get; private set; }
        public List<Role> Roles { get; private set; }
        public List<Champion> Champions { get; private set; }
        public Region Region { get; private set; }

        public Coach(int id, ulong coachid, List<Role> roles, List<Champion> champions, Region region)
        {
            this.Id = id;
            this.CoachId = coachid;
            this.Roles = roles;
            this.Champions = champions;
            this.Region = region;
        }
    }
}
