using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using Discord;
using Discord.Commands;
using Languages;

namespace AtlasBot.Modules.Server_Info
{
    public class ServerInfoCommands
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public ServerInfoCommands(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public void InviteLink()
        {
            commands.CreateCommand("InviteLink")
                .Parameter("InviteLink", ParameterType.Required)
                .Do(async (e) =>
                {
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        if (e.GetArg("InviteLink").Contains("discord.gg/"))
                        {
                            new ServerRepo(new ServerContext()).AddInviteLink(e.User.Id, e.Server.Id,
                                e.GetArg("InviteLink"));
                            await e.Channel.SendMessage(Eng_Default.InviteLinkSet(e.GetArg("InviteLink")));
                        }
                        else
                        {
                            await e.Channel.SendMessage("Invalid invite link");
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessage(Eng_Default.ServerIsNotVerified());
                    }

                });
        }

        public void Description()
        {
            commands.CreateCommand("Description")
                .Parameter("Description", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id) && new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id))
                    {
                        if (e.GetArg("Description") != "")
                        {
                            if (e.GetArg("Description").Length <= 500)
                            {
                                new ServerRepo(new ServerContext()).SetServerGescriptiong(e.Server.Id, (e.GetArg("Description")));
                                Eng_Default.DescriptionSet();
                            }
                            else
                            {
                                returnstring = Eng_Default.DescriptionTooLong();
                            }
                        }
                        else
                        {
                            returnstring = new ServerRepo(new ServerContext()).GetServerDescription(e.Server.Id);
                        }
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }

        public void ServerInfo()
        {
            commands.CreateCommand("Servers")
                .Parameter("CommandType", ParameterType.Optional)
                .Parameter("Filter", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    if (e.GetArg("Filter") == "" && e.GetArg("CommandType").ToLower() == "list")
                    {
                        returnstring = "Servers that use AtlasBot:\n```";
                        int count = 0;
                        foreach (Discord.Server server in BotUser.Servers)
                        {
                            if (count != BotUser.Servers.Count() - 1)
                            {
                                returnstring += new ServerRepo(new ServerContext()).ServerName(server.Id) + ", ";
                            }
                            else
                            {
                                returnstring += new ServerRepo(new ServerContext()).ServerName(server.Id) + ".";
                            }
                            count++;

                        }
                        returnstring += "```";
                    }
                    if (e.GetArg("CommandType").ToLower() == "info" && e.GetArg("Filter") != "")
                    {
                        foreach (Discord.Server server in BotUser.Servers)
                        {
                            if (new ServerRepo(new ServerContext()).ServerName(server.Id).ToLower() ==
                                e.GetArg("Filter").ToLower())
                            {
                                returnstring = "**" + new ServerRepo(new ServerContext()).ServerName(server.Id) + ":** ";
                                returnstring += "\n" +
                                new ServerRepo(new ServerContext()).GetServerDescription(server.Id) +
                                "\nInvite link: " + new ServerRepo(new ServerContext()).InviteLink(server.Id);
                            }
                        }
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
