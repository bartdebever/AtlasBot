using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord.Commands;
using Languages;

namespace AtlasBot.Modules.Coach
{
    public class CoachCommands
    {
        private CommandService commands;

        public CoachCommands(CommandService commands)
        {
            this.commands = commands;
        }

        public void CreateCommands()
        {
            AddCoach();
        }

        private void AddCoach()
        {
            commands.CreateCommand("AddCoach")
                .Parameter("CommandType")
                .Parameter("User")
                .Parameter("Role", ParameterType.Optional)
                .Parameter("ChampionName",ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    CoachTrigger trigger = new CoachTrigger(commands);
                    if (e.Server.Id == 302775478824075267 && serverRepo.IsAdmin(e.User.Id, e.Server.Id))
                    {
                        if (e.GetArg("CommandType") == "add")
                        {
                            try
                            {
                                trigger.AddCoach(e.Message.MentionedUsers.First().Id, e.GetArg("ChampionName"),
                                    e.GetArg("Role"));
                                returnstring = "Coach added successfully.";
                            }
                            catch
                            {
                                returnstring = "Failed to add coach";
                            }
                        }
                    }
                    else
                    {
                        returnstring = Eng_Default.NotAllowed();
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
