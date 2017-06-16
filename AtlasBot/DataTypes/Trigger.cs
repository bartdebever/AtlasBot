using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace AtlasBot.DataTypes
{
    public abstract class Trigger
    {
        public DiscordClient BotUser;

        public Trigger(DiscordClient botUser)
        {
            this.BotUser = botUser;
        }

        public virtual void CreateTriggers()
        {
            
        }
    }
}
