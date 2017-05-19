using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace AtlasBot.DataTypes
{
    public abstract class Commands
    {
        public  DiscordClient BotUser { get; private set; }

        public Commands(DiscordClient BotUser)
        {
            this.BotUser = BotUser;
        }

        public abstract void CreateCommands();
    }
}
