using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.Logging;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Languages;

namespace AtlasBot.Modules.Server
{
    public class ServerManagement
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public ServerManagement(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public void ServerAdded()
        {
            BotUser.JoinedServer += async (s, u) =>
            {
                AddServer(u.Server);
                await u.Server.DefaultChannel.SendMessage("New server has been detected!");
            };
        }

        public void VerifyServer()
        {
            commands.CreateCommand("verify")
                .Parameter("Key", ParameterType.Required)
                .Do(async (e) =>
                {
                    new ServerRepo(new ServerContext()).VerifyServerSQL(e.User.Id, e.GetArg("Key"));
                    await e.Channel.SendMessage(Eng_Default.ServerVerified());
                });
        }

        public void CheckForNewServer()
        {
            BotUser.ServerAvailable += async (s, u) =>
            {
                foreach (Discord.Server server in BotUser.Servers)
                {
                    bool found = false;
                    foreach (ulong id in new ServerRepo(new ServerContext()).GetAllServerIds())
                    {
                        if (server.Id == id)
                        {
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        try
                        {
                            await server.DefaultChannel.SendMessage("New server found");
                        }
                        catch { }

                        AddServer(server);
                    }

                }
            };
        }

        public async void AddServer(Discord.Server server)
        {
            ulong serverid = server.Id;
            ulong ownerid = server.Owner.Id;
            string servername = server.Name;
            string key = new Program.Bot().RandomStringGenerator();
            new ServerRepo(new ServerContext()).AddServer(serverid, ownerid, servername, key);
            Console.WriteLine(servername + " has added AtlasBot to their server");
            new Log(BotUser, commands).AdminLog(servername + " has added the bot. Owner: " + server.Owner.ToString());
            new Log(BotUser, commands).DMBort(servername + ": " + server.Owner.ToString() + " Key: " + key);
            await server.Owner.SendMessage(Eng_Default.VerifyServer());
            new SettingsRepo(new SettingsContext()).CreateSettings(serverid);

        }
    }
}
