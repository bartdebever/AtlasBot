using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Languages;

namespace AtlasBot.Modules.Administrative
{
    public class BotManagement
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public BotManagement(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }

        public void SetGame()
        {
            commands.CreateCommand("SetGame")
                .Parameter("Game", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    UserRepo userRepo = new UserRepo(new UserContext());
                    if (userRepo.IsAtlasAdmin(e.User.Id))
                    {
                        BotUser.SetGame(e.GetArg("Game"));
                        await e.Channel.SendMessage("Game set.");
                    }
                    else
                    {
                        await e.Channel.SendMessage(Eng_Default.NotAllowed());
                    }
                });
        }
    }
}
