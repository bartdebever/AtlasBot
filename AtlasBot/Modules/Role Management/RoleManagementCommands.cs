using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.Rank;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Languages;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Role_Management
{
    public class RoleManagementCommands
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public RoleManagementCommands(DiscordClient client, CommandService commands)
        {
            this.BotUser = client;
            this.commands = commands;
        }
        public void Update()
        {
            commands.CreateCommand("Update")
            .Do(async (e) =>
            {
                RankCommands rankCommands = new RankCommands(BotUser, commands);
                string returnstring = "";
                ServerRepo serverRepo = new ServerRepo(new ServerContext());
                UserRepo userRepo = new UserRepo(new UserContext());
                RoleManagementTrigger rmt = new RoleManagementTrigger(BotUser, commands);
                if (userRepo.IsAtlasAdmin(e.User.Id))
                {
                    foreach (Discord.Server server in BotUser.Servers)
                    {
                        if (serverRepo.IsServerVerified(server.Id))
                        {
                            foreach (Discord.User user in server.Users)
                            {

                                try
                                {
                                    await Task.Run(() => rmt.RemoveRoles(server, user));
                                    await Task.Run(() => rankCommands.GetRankRoles(server, user));
                                }
                                catch
                                {

                                }

                            }
                        }
                    }
                    returnstring = "System update complete.";
                }

                else if (new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id))
                {
                    if ((serverRepo.GetLastupdateDateServer(e.Server.Id) < DateTime.Today) && serverRepo.IsServerVerified(e.Server.Id))
                    {
                        foreach (Discord.User user in e.Server.Users)
                        {
                            try
                            {
                                await Task.Run(() => rmt.RemoveRoles(e.Server, user));
                                await Task.Run(() => rankCommands.GetRankRoles(e.Server, user));
                            }
                            catch
                            {
                                Console.WriteLine("Failed to give roles, account not registered.");
                            }


                        }
                        serverRepo.SetUpdateDateServer(e.Server.Id, DateTime.Today);
                        returnstring = "Server update successfull.";
                    }
                    else
                    {
                        returnstring = "Please wait for one day to update your server again.";
                    }

                }
                else
                {
                    if (userRepo.GetLastRefreshDate(e.User.Id) > DateTime.Today && serverRepo.IsServerVerified(e.Server.Id))
                    {
                        try
                        {
                            await Task.Run(() => rmt.RemoveRoles(e.Server, e.User));
                            await Task.Run(() => rankCommands.GetRankRoles(e.Server, e.User));
                        }
                        catch
                        {

                        }
                        userRepo.SetLastRefreshDate(e.User.Id, DateTime.Now);
                    }
                }
                await e.Channel.SendMessage(returnstring);
            });

        }
        public void GetRoles()
        {
            commands.CreateCommand("GetRoles")
                .Do(async (e) =>
                {
                    await Task.Run(() => new RoleManagementTrigger(BotUser, commands).RemoveRoles(e.Server, e.User));
                    try
                    {

                        await Task.Run(() => GetRoles(e.Server, e.User));
                        await e.Channel.SendMessage(Eng_Default.RolesHaveBeenGiven());
                    }
                    catch
                    {
                        await e.Channel.SendMessage(Eng_Default.RegisterAccount());
                    }



                });
        }
        public async void GetRoles(Discord.Server server, Discord.User discorduser)
        {
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            if (settingsRepo.RankByAccount(server.Id) == true)
            {
                Summoner summoner = null;
                try
                {
                    DataLibary.Models.User user =
                        new UserRepo(new UserContext()).GetUserByDiscord(discorduser.Id);
                    summoner =
                        new SummonerAPI().GetSummoner(
                            new SummonerRepo(new SummonerContext()).GetSummonerByUserId(user),
                            ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                                new RegionRepo(new RegionContext()).GetRegionId(user)
                            ));
                }
                catch
                {

                }
                //summoner will be null when the item does not excist within the database.
                //This is only done so there will be a proper returnmessage send to the user.
                if (summoner != null)
                {
                    if (settingsRepo.RankCommandType(server.Id) == CommandType.Basic)
                    {
                        string rank = new RankAPI().GetRankingSimple(summoner,
                            Queue.RankedSolo5x5);
                        try
                        {
                            await discorduser.AddRoles(
                                 server.GetRole(settingsRepo.GetOverride(rank.ToLower(),
                                     server.Id)));
                        }
                        catch
                        {
                            await discorduser.AddRoles(server.FindRoles(rank, false).First());
                        }

                    }
                    else if (settingsRepo.RankCommandType(server.Id) == CommandType.Division)
                    {
                        string rank =
                            new RankAPI().GetRankingHarder(summoner, Queue.RankedSolo5x5)
                                .ToLower();
                        try
                        {
                            await discorduser.AddRoles(
                                server.GetRole(settingsRepo.GetOverride(rank, server.Id)));
                        }
                        catch
                        {
                            await discorduser.AddRoles(server.FindRoles(rank, false).First());
                        }


                    }
                    else if (settingsRepo.RankCommandType(server.Id) == CommandType.PerQueue)
                    {
                        //Each of these can fail when someone does not have this rank, therefore this isn't in one big Try because it could fail halfway.
                        try
                        {
                            string rank = "Solo " +
                                          new RankAPI().GetRankingSimple(summoner,
                                              Queue.RankedSolo5x5);
                            try
                            {
                                await discorduser.AddRoles(
                                    server.GetRole(settingsRepo.GetOverride(rank, server.Id)));
                            }
                            catch
                            {
                                await discorduser.AddRoles(server.FindRoles(rank, false).First());
                            }
                        }
                        catch
                        {
                            Console.WriteLine(discorduser.Name + "doesn't have a soloq rank");
                        }
                        try
                        {
                            string rank = "Flex " +
                                          new RankAPI().GetRankingSimple(summoner,
                                              Queue.RankedFlexSR);
                            try
                            {
                                await discorduser.AddRoles(
                                    server.GetRole(settingsRepo.GetOverride(rank, server.Id)));
                            }
                            catch
                            {
                                await discorduser.AddRoles(server.FindRoles(rank, false).First());
                            }
                        }
                        catch
                        {
                            Console.WriteLine(discorduser.Name + "doesn't have a flex rank");
                        }
                        try
                        {
                            string rank = "3v3 " +
                                          new RankAPI().GetRankingSimple(summoner,
                                              Queue.RankedFlexTT);
                            try
                            {
                                await discorduser.AddRoles(
                                    server.GetRole(settingsRepo.GetOverride(rank, server.Id)));
                            }
                            catch
                            {
                                await discorduser.AddRoles(server.FindRoles(rank, false).First());
                            }
                        }
                        catch
                        {
                            Console.WriteLine(discorduser.Name + "doesn't have a 3v3 rank");
                        }

                    }
                }
            }
            if (settingsRepo.RegionByAccount(server.Id))
            {
                Summoner summoner = null;
                try
                {
                    DataLibary.Models.User user =
                        new UserRepo(new UserContext()).GetUserByDiscord(discorduser.Id);
                    summoner =
                        new SummonerAPI().GetSummoner(
                            new SummonerRepo(new SummonerContext()).GetSummonerByUserId(user),
                            ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                                new RegionRepo(new RegionContext()).GetRegionId(user)
                            ));
                }
                catch
                {

                }
                //summoner will be null when the item does not excist within the database.
                //This is only done so there will be a proper returnmessage send to the user.
                if (summoner != null)
                {
                    foreach (string region in new RegionRepo(new RegionContext()).GetAllRegions())
                    {
                        if (region.ToLower() == summoner.Region.ToString().ToLower())
                        {
                            try
                            {
                                await discorduser.AddRoles(
                                    server.GetRole(settingsRepo.GetOverride(region.ToLower(), server.Id)));

                            }
                            catch
                            {
                                await discorduser.AddRoles(server.FindRoles(region, false).First());

                            }
                        }

                    }
                }
            }
            if (settingsRepo.RoleByAccount(server.Id))
            {
                List<string> filter = new List<string>();
                if (settingsRepo.RoleCommandType(server.Id) == CommandType.Basic)
                {
                    filter = DataLibary.Models.Roles.NormalRoles();
                }
                else if (settingsRepo.RoleCommandType(server.Id) == CommandType.Main)
                {
                    filter = DataLibary.Models.Roles.MainRoles();
                }
                else if (settingsRepo.RoleCommandType(server.Id) == CommandType.Mains)
                {
                    filter = DataLibary.Models.Roles.MainsRoles();
                }

                Summoner summoner = null;
                try
                {
                    DataLibary.Models.User user =
                        new UserRepo(new UserContext()).GetUserByDiscord(discorduser.Id);
                    summoner =
                        new SummonerAPI().GetSummoner(
                            new SummonerRepo(new SummonerContext()).GetSummonerByUserId(user),
                            ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                                new RegionRepo(new RegionContext()).GetRegionId(user)
                            ));
                }
                catch
                {

                }
                //summoner will be null when the item does not excist within the database.
                //This is only done so there will be a proper returnmessage send to the user.
                if (summoner != null)
                {
                    try
                    {
                        string mainrole = new RoleAPI().GetRole(summoner);
                        foreach (string role in filter)
                        {
                            if (role.Contains(mainrole))
                            {
                                try
                                {
                                    ulong id = settingsRepo.GetOverride(role.ToLower(), server.Id);
                                    await discorduser.AddRoles(server.GetRole(id));

                                }
                                catch
                                {
                                    await discorduser.AddRoles(server.FindRoles(role, false).First());

                                }
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error in roles");
                    }


                }

            }
        }
    }
}
