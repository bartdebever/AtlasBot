using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace AtlasBot.Modules.Administrative
{
    public class BotTriggers
    {
        private DiscordClient BotUser;

        public BotTriggers(DiscordClient BotUser)
        {
            this.BotUser = BotUser;
        }
            public void SetGame(string game)
            {
                BotUser.SetGame(game);
            }
        
    }
}
