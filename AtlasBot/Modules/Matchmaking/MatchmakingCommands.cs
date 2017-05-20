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
using Keys;
using Languages;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Matchmaking
{
    public class MatchmakingCommands
    {
        private CommandService commands;
        private DiscordClient BotUser;
        private MatchmakingTrigger trigger;

        public MatchmakingCommands(CommandService commands, DiscordClient BotUser, MatchmakingTrigger trigger)
        {
            this.commands = commands;
            this.BotUser = BotUser;
            this.trigger = trigger;
        }

        public void CreateCommands()
        {
            QueueUp();
            LeaveQueue();
            ClearQueues();
        }

        private void ClearQueues()
        {
            commands.CreateCommand("ClearQueue")
                .Do(async (e) =>
                {
                    UserRepo userRepo = new UserRepo(new UserContext());
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    if (userRepo.IsAtlasAdmin(e.User.Id))
                    {
                        foreach (var server in BotUser.Servers)
                        {
                            if (settingsRepo.lfgStatus(server.Id) || server.Id == DiscordIds.AtlasId)
                            {
                                await (Task.Run(() => trigger.RemoveMessages(server)));
                                
                            }
                        }
                        await e.Channel.SendMessage("Done!");
                    }
                });
        }
        private void QueueUp()
        {
            commands.CreateCommand("QueueUp")
                .Alias("Queue")
                 .Parameter("Queue", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    StringHandler SH = new StringHandler(e.Server);
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    if (serverRepo.IsServerVerified(e.Server.Id)&& settingsRepo.lfgStatus(e.Server.Id) || e.Server.Id == DiscordIds.AtlasId)
                    {
                        Summoner summoner = null;
                        try
                        {
                            DataLibary.Models.User user =
                                new UserRepo(new UserContext()).GetUserByDiscord(e.User.Id);
                            summoner =
                                new SummonerAPI().GetSummoner(
                                    new SummonerRepo(new SummonerContext()).GetSummonerByUserId(user),
                                    ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                                        new RegionRepo(new RegionContext()).GetRegionId(user)
                                    ));
                        }
                        catch
                        {
                            returnstring = Eng_Default.RegisterAccount();
                        }
                        if (summoner != null)
                        {
                            try
                            {
                                bool found = false;
                                string queue = "";
                                foreach (string line in Queues.GetQueues())
                                {
                                    if (e.GetArg("Queue").ToLower() == line)
                                    {
                                        found = true;
                                        queue = line;
                                    }
                                }
                                if (found == true)
                                {
                                    trigger.QueuePerson(summoner, e.User, e.Server, queue);
                                    returnstring = SH.Build("QUP1");
                                    //returnstring = "You were queued!";
                                }
                                else
                                {
                                    returnstring = "Please enter one of the following queues: ";
                                    foreach (string line in Queues.GetQueues())
                                    {
                                        returnstring += "\n -" + line;
                                    }
                                }
                                
                            }
                            catch
                            {
                                returnstring = "You are already queued.";
                            }
                        }
                        
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }

        private void LeaveQueue()
        {
            commands.CreateCommand("LeaveQueue")
                .Do(async (e) =>
                {
                    string reaction = "";
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    if (serverRepo.IsServerVerified(e.Server.Id) && settingsRepo.lfgStatus(e.Server.Id))
                    {
                        try
                        {
                            trigger.LeaveQueue(e.User);
                            reaction = "You have left queue.";
                        }
                        catch
                        {
                            reaction = "Could not find you in the queue.";
                        }
                    }
                    else
                    {
                        
                    }
                    await e.Channel.SendMessage(reaction);
                });
        }
    }
}
