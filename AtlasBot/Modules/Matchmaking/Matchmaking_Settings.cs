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

namespace AtlasBot.Modules.Matchmaking
{
    public class Matchmaking_Settings
    {
        private CommandService commands;

        public Matchmaking_Settings(CommandService commands)
        {
            this.commands = commands;
        }

        public void ChannelSettings()
        {
            commands.CreateCommand("LFGChannel")
                .Parameter("CommandType")
                .Parameter("Channel", ParameterType.Optional)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    if (serverRepo.IsServerVerified(e.Server.Id) && serverRepo.IsAdmin(e.User.Id, e.Server.Id))
                    {
                        if (e.GetArg("CommandType").ToLower() == "set")
                        {
                            try
                            {
                                settingsRepo.SetLfgChannel(e.Message.MentionedChannels.First().Id, e.Server.Id);
                                returnstring = "Looking for group channel has been set.";
                            }
                            catch
                            {
                                returnstring = "Failed to set a channel, have you tagged a channel?";
                            }
                        }
                        else if (e.GetArg("CommandType").ToLower() == "disable")
                        {
                            settingsRepo.DisableLfg(e.Server.Id);
                            returnstring = "Looking for group has been disabled.";
                        }
                    }
                    else
                    {
                        returnstring = Eng_Default.ServerIsNotVerified();
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
