using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace AtlasBot.Modules.Fun
{
    public class Interaction
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public Interaction(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
            CreateCommands();
        }

        public void CreateCommands()
        {
            WelcomeMessageByBot();
        }
        private void WelcomeMessageByBot()
        {
            commands.CreateCommand("")
                .Parameter("something", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    if (e.User.IsBot)
                    {
                        await e.Channel.SendMessage("Hello fellow bot, it's time to take over this server!");
                    }
                });
        }
    }
}
