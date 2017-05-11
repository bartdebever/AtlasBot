using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.Role_Management;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Languages;
using RiotLibary.Roles;
using RiotSharp.SummonerEndpoint;
using RiotSharp;
using Queue = RiotSharp.Queue;

namespace AtlasBot.Modules.Rank
{
    public class RankCommands
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public RankCommands(DiscordClient bot, CommandService commands)
        {
            this.BotUser = bot;
            this.commands = commands;
            GetRank();
        }

        public void GetRank()
        {
            commands.CreateCommand("Rank")
                .Parameter("rank", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring = "error";
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        try
                        {
                            new RoleManagementTrigger(BotUser, commands).OverrideDeletion(e.Server);
                        }
                        catch
                        {
                        }
                        SettingsRepo settingsRepo = (new SettingsRepo(new SettingsContext()));
                        if (e.GetArg("rank").Split(' ').First() == "delete" ||
                            e.GetArg("rank").Split(' ').First() == "remove")
                        {
                            foreach (string region in Ranks.BasicRanks())
                            {
                                if (region.ToLower() ==
                                    e.GetArg("rank")
                                        .Substring(e.GetArg("rank").IndexOf(" ") + 1,
                                            e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1)
                                        .ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id),
                                            e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = Eng_Default.RoleHasBeenRemoved(region);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            foreach (string region in Ranks.QueueRanks())
                            {
                                if (region.ToLower() ==
                                    e.GetArg("rank")
                                        .Substring(e.GetArg("rank").IndexOf(" ") + 1,
                                            e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1)
                                        .ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id),
                                            e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = Eng_Default.RoleHasBeenRemoved(region);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            foreach (string region in Ranks.DivisionRanks())
                            {
                                if (region.ToLower() ==
                                    e.GetArg("rank")
                                        .Substring(e.GetArg("rank").IndexOf(" ") + 1,
                                            e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1)
                                        .ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id),
                                            e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = Eng_Default.RoleHasBeenRemoved(region);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            try
                            {
                                foreach (string role in settingsRepo.GetAllOverrides(e.Server.Id))
                                {
                                    var replacement = e.Server.GetRole(Convert.ToUInt64(role.Split(':').Last()));
                                    if (
                                        e.GetArg("rank")
                                            .Substring(e.GetArg("rank").IndexOf(" ") + 1,
                                                e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1)
                                            .ToLower() == replacement.Name.ToLower())
                                    {
                                        await e.User.RemoveRoles(replacement);
                                        returnstring = Eng_Default.RoleHasBeenRemoved(role);
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        else if (e.GetArg("rank").ToLower() == "list")
                        {
                            if (settingsRepo.RankByAccount(e.Server.Id) == true ||
                                settingsRepo.RankByParameter(e.Server.Id) == true)
                            {
                                returnstring = "Assignable roles on this server:";
                                if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Basic)
                                {
                                    foreach (string r in Ranks.BasicRanks())
                                    {
                                        returnstring += "\n- " + r;
                                    }
                                }
                                else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.PerQueue)
                                {
                                    foreach (string r in Ranks.QueueRanks())
                                    {
                                        returnstring += "\n- " + r;
                                    }
                                }
                                else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Division)
                                {
                                    foreach (string r in Ranks.BasicRanks())
                                    {
                                        returnstring += "\n- " + r + " V to I";
                                    }
                                }
                            }
                            else
                            {
                                returnstring = Eng_Default.ServerDoesNotAllow();
                            }
                        }
                        else if (e.GetArg("rank") == "?" || e.GetArg("rank").ToLower() == "help")
                        {
                            returnstring = "Use the base command -rank to get a rank assigned as your role.";
                            if (settingsRepo.RankByAccount(e.Server.Id) == true)
                            {
                                returnstring +=
                                    "\nYou can use *-Rank* to get your ranks based on bound league of legends account.";
                            }
                            if (settingsRepo.RankByParameter(e.Server.Id) == true)
                            {
                                returnstring +=
                                    "\nYou can use *-Rank <League rank>* to get a role based on your input";
                                returnstring += "\nRoles you can get on this server are:";
                                if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Basic)
                                {
                                    foreach (string r in Ranks.BasicRanks())
                                    {
                                        returnstring += "\n- " + r;
                                    }
                                }
                                else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.PerQueue)
                                {
                                    foreach (string r in Ranks.QueueRanks())
                                    {
                                        returnstring += "\n- " + r;
                                    }
                                }
                                else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Division)
                                {
                                    foreach (string r in Ranks.BasicRanks())
                                    {
                                        returnstring += "\n- " + r + " V to I";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (e.GetArg("rank") == "")
                            {
                                //Checks if gettings ranks by account is disabled (Unsure why someone would disable this but hey ¯\_(ツ)_/¯ someone might want so)
                                if (settingsRepo.RankByAccount(e.Server.Id) == true)
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
                                        returnstring =
                                            Eng_Default.RegisterAccount();
                                    }
                                    //summoner will be null when the item does not excist within the database.
                                    //This is only done so there will be a proper returnmessage send to the user.
                                    if (summoner != null)
                                    {
                                        if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Basic)
                                        {
                                            string rank = new RankAPI().GetRankingSimple(summoner,
                                                Queue.RankedSolo5x5);
                                            try
                                            {
                                                await e.User.AddRoles(
                                                    e.Server.GetRole(settingsRepo.GetOverride(rank.ToLower(),
                                                        e.Server.Id)));
                                            }
                                            catch
                                            {
                                                await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                            }
                                            returnstring = Eng_Default.RoleHasBeenGiven(rank);
                                        }
                                        else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Division)
                                        {
                                            string rank =
                                                new RankAPI().GetRankingHarder(summoner, Queue.RankedSolo5x5)
                                                    .ToLower();
                                            try
                                            {
                                                await e.User.AddRoles(
                                                    e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id)));
                                            }
                                            catch
                                            {
                                                await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                            }

                                            returnstring = Eng_Default.RoleHasBeenGiven(rank);
                                        }
                                        else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.PerQueue)
                                        {
                                            //Each of these can fail when someone does not have this rank, therefore this isn't in one big Try because it could fail halfway.
                                            try
                                            {
                                                string rank = "Solo " +
                                                              new RankAPI().GetRankingSimple(summoner,
                                                                  Queue.RankedSolo5x5);
                                                try
                                                {
                                                    await e.User.AddRoles(
                                                        e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id)));
                                                }
                                                catch
                                                {
                                                    await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine(e.User.Name + "doesn't have a soloq rank");
                                            }
                                            try
                                            {
                                                string rank = "Flex " +
                                                              new RankAPI().GetRankingSimple(summoner,
                                                                  Queue.RankedFlexSR);
                                                try
                                                {
                                                    await e.User.AddRoles(
                                                        e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id)));
                                                }
                                                catch
                                                {
                                                    await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine(e.User.Name + "doesn't have a flex rank");
                                            }
                                            try
                                            {
                                                string rank = "3v3 " +
                                                              new RankAPI().GetRankingSimple(summoner,
                                                                  Queue.RankedFlexTT);
                                                try
                                                {
                                                    await e.User.AddRoles(
                                                        e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id)));
                                                }
                                                catch
                                                {
                                                    await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine(e.User.Name + "doesn't have a 3v3 rank");
                                            }
                                            returnstring = Eng_Default.RolesHaveBeenGiven();
                                        }
                                    }

                                }
                                else
                                {
                                    returnstring = Eng_Default.ServerDoesNotAllow();
                                }
                            }
                            else
                            {
                                //Check settings and give ranks according to the parameter
                                if (settingsRepo.RankByParameter(e.Server.Id) == true)
                                {
                                    bool found = false;
                                    List<string> filter = new List<string>();
                                    if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Basic)
                                    {
                                        filter = Ranks.BasicRanks();
                                    }

                                    else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.PerQueue)
                                    {
                                        filter = Ranks.QueueRanks();
                                    }
                                    foreach (string rank in filter)
                                    {
                                        if (e.GetArg("rank").ToLower() == rank.ToLower())
                                        {
                                            try
                                            {
                                                Discord.Role r =
                                                    e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id));
                                                if (!settingsRepo.IsRoleDisabled(r.Name.ToLower(), e.Server.Id))
                                                {
                                                    await e.User.AddRoles(r);
                                                    returnstring = Eng_Default.RoleHasBeenGiven(r.Name);
                                                }
                                                else
                                                {
                                                    returnstring = Eng_Default.RoleHasBeenDisabled();
                                                }
                                            }
                                            catch
                                            {
                                                Discord.Role r = e.Server.FindRoles(rank, false).First();
                                                if (!settingsRepo.IsRoleDisabled(r.Name.ToLower(), e.Server.Id))
                                                {
                                                    await e.User.AddRoles(r);
                                                    returnstring = Eng_Default.RoleHasBeenGiven(r.Name);
                                                }
                                                else
                                                {
                                                    returnstring = Eng_Default.RoleHasBeenDisabled();
                                                }

                                            }

                                            found = true;
                                        }
                                    }
                                    if (found == false)
                                    {
                                        returnstring = Eng_Default.RoleNotFound(e.GetArg("rank"));
                                    }
                                }
                                else
                                {
                                    returnstring = Eng_Default.ServerDoesNotAllow();
                                }
                            }
                        }
                    }
                    else
                    {
                        returnstring = Eng_Default.ServerIsNotVerified();
                    }


                    await e.Channel.SendMessage(returnstring);
                });
        }
        public async void GetRankRoles(Discord.Server server, Discord.User discorduser)
        {
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            if

            (settingsRepo.RankByAccount
                 (server.Id)
             == true)
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
        }
    }
}        
    
