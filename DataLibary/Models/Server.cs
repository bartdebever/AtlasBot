using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Models
{
    public class Server
    {
        public int Id { get; private set; }
        public ulong DiscordId { get; private set; }
        public string InviteLink { get; private set; }
        public User OwnerUser { get; private set; }
        public string ServerName { get; private set; }
        public string Description { get; private set; }
        public Setting Settings { get; private set; }

        public Server(int id, ulong discordid, string invitelink, User owner, string servername, string discription,
            Setting settings)
        {
            this.Id = id;
            this.DiscordId = discordid;
            this.InviteLink = invitelink;
            this.OwnerUser = owner;
            this.ServerName = servername;
            this.Description = discription;
            this.Settings = settings;
        }
    }
}
