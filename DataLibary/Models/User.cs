using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;

namespace DataLibary.Models
{
    public class User
    {
        public int Id { get; private set; }
        public ulong DiscordId { get; private set; }
        public int RiotId { get; private set; }
        public Region Region { get; private set; }

        public User(int id, ulong discordId, int riotid, Region region)
        {
            this.Id = id;
            this.DiscordId = discordId;
            this.RiotId = riotid;
            this.Region = region;
        }
    }
}
