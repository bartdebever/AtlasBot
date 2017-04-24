using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Models
{
    public class AtlasAdmin
    {
        public int Id { get; private set; }
        public ulong DiscordId { get; private set; }

        public AtlasAdmin(int id, ulong discordId)
        {
            this.Id = id;
            this.DiscordId = discordId;
        }
    }
}
