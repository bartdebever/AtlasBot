using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.DataTypes;
using Discord;

namespace AtlasBot.Modules.Administrative
{
    public class BotTriggers : Trigger
    {

        public BotTriggers(DiscordClient BotUser):base(BotUser)
        {
           
        }
        public void SetGame(string game)
        {
            BotUser.SetGame(game);
        }
    }
}
