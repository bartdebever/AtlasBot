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
using RiotLibary.Roles;
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

        public void ClearQueues()
        {
            commands.CreateCommand("ClearQueue")
                .Do(async (e) =>
                {
                    UserRepo userRepo = new UserRepo(new UserContext());
                    if (userRepo.IsAtlasAdmin(e.User.Id))
                    {
                        SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                        foreach (var server in BotUser.Servers)
                        {
                            if (settingsRepo.lfgStatus(server.Id))
                            {
                                trigger.RemoveMessages(server);
                                await e.Channel.SendMessage("Done!");
                            }
                        }
                    }
                });
        }
        private void QueueUp()
        {
            commands.CreateCommand("QueueUp")
                .Do(async (e) =>
                {
                    string returnstring = "";
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    if (serverRepo.IsServerVerified(e.Server.Id)&& settingsRepo.lfgStatus(e.Server.Id))
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
                                trigger.QueuePerson(summoner, e.User, e.Server);
                                returnstring = "You were queued!";
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
