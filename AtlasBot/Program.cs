﻿using System;
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
using Nito.AsyncEx;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;
using ToolKit;
using Region = RiotSharp.Region;
using Role = Discord.API.Client.Role;

namespace AtlasBot
{
    public class Program
    {

        public static void Main(string[] args)
        {
            new Bot();
        }

        public class Bot
        {
            DiscordClient BotUser;
            CommandService commands;

            public Bot()
            {

                BotUser = new DiscordClient(x =>
                {
                    x.LogLevel = LogSeverity.Info;

                });
                BotUser.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

                BotUser.UsingCommands(x =>
                {
                    x.PrefixChar = '-';
                    x.AllowMentionPrefix = true;

                });
                commands = BotUser.GetService<CommandService>();
                ServerAdded();
                VerifyServer();
                InviteLink();
                ClaimAccount();
                GetRank();
                GetRegion();
                ChangeType();
                ChangeCommandAllowed();
                OverrideSystem();
                CheckForNewServer();
                BotUser.ExecuteAndWait(async () =>
                {
                    await BotUser.Connect(Keys.Keys.discordKey, TokenType.Bot);
                });
            }

            #region ServerJoining

            private void ServerAdded()
            {
                BotUser.JoinedServer += async (s, u) =>
                {
                    AddServer(u.Server);
                    await u.Server.DefaultChannel.SendMessage("New server has been detected!");
                };
            }

            private void VerifyServer()
            {
                commands.CreateCommand("verify")
                    .Parameter("Key", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        new ServerRepo(new ServerContext()).VerifyServerSQL(e.User.Id, e.GetArg("Key"));
                        await e.Channel.SendMessage(Eng_Default.ServerVerified());
                    });
            }

            private void CheckForNewServer()
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

            private void AddServer(Discord.Server server)
            {
                    ulong serverid = server.Id;
                    ulong ownerid = server.Owner.Id;
                    string servername = server.Name;
                    string key = RandomStringGenerator();
                    new ServerRepo(new ServerContext()).AddServer(serverid, ownerid, servername, key);
                    Console.WriteLine(servername + " has added AtlasBot to their server");
                    AdminLog(servername + " has added the bot. Owner: " + server.Owner.ToString());
                    DMBort(servername + ": " + server.Owner.ToString() + " Key: " + key);
                    server.Owner.SendMessage(Eng_Default.VerifyServer());
                new SettingsRepo(new SettingsContext()).CreateSettings(serverid);

            }
            #endregion ServerJoining

            #region ServerManagement
            private void InviteLink()
            {
                commands.CreateCommand("SetInvite")
                    .Parameter("InviteLink", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                        {
                            new ServerRepo(new ServerContext()).AddInviteLink(e.User.Id, e.Server.Id,
                                e.GetArg("InviteLink"));
                            await e.Channel.SendMessage(Eng_Default.InviteLinkSet(e.GetArg("InviteLink")));
                        }
                        else
                        {
                            await e.Channel.SendMessage(Eng_Default.ServerIsNotVerified());
                        }
                        
                    });
            }

            private void ChangeType()
            {
                commands.CreateCommand("CommandType")
                    .Parameter("Type", ParameterType.Required)
                    .Parameter("CommandType", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        string returnstring = "error";
                        if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                        {
                            
                            if (new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id) == true)
                            {
                                if (e.GetArg("Type").ToLower() == "rank")
                                {
                                    CommandType result;
                                    CommandType.TryParse(e.GetArg("CommandType"), out result);
                                    new SettingsRepo(new SettingsContext()).SetRankType(result, e.Server.Id);
                                    returnstring = Eng_Default.CommandTypeChange("rank", e.GetArg("CommandType"));
                                }
                            }
                            else
                            {
                                returnstring = Eng_Default.NotAllowed();
                            }
                        }
                        else
                        {
                            
                        }
                            
                        await e.Channel.SendMessage(returnstring);
                    });
            }

            private void ChangeCommandAllowed()
            {
                commands.CreateCommand("Command")
                    .Parameter("Command", ParameterType.Required)
                    .Parameter("Value", ParameterType.Optional)
                    .Do(async (e)=>
                {

                    string returnstring = "error";
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        if (e.GetArg("Command").ToLower() == "help" || e.GetArg("Command").ToLower() == "?")
                        {
                            returnstring =
                                "Use this command to change the behavior of your server. You can allow and deny the following features:" +
                                "\n- Regionaccount: Allows the user to type -region to get a region role assigned." +
                                "\n- Regionparameter: Allow the user to type -region <region> to get a region role assinged." +
                                "\n- Rankaccount: Allow the user to type -rank to get a rank role assinged." +
                                "\n- Rankparameter: Allow the user to type -rank <rank> to get a rank role assinged" +
                                "\n\nPlease use the format -Command <Command> <Value>, example: *-Command rankaccount true*";
                        }
                        else
                        {
                            bool value = false;
                            if (bool.TryParse(e.GetArg("Value"), out value))
                            {
                                if (new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id))
                                {
                                    if (e.GetArg("Command").ToLower() == "rankaccount")
                                    {

                                        new SettingsRepo(new SettingsContext()).ToggleAccountRank(value, e.Server.Id);
                                        returnstring = Eng_Default.CommandPermsChanged("Rank by account",
                                            value.ToString());
                                    }
                                    else if (e.GetArg("Command").ToLower() == "rankparameter")
                                    {
                                        new SettingsRepo(new SettingsContext()).ToggleRankParameter(value, e.Server.Id);
                                        returnstring = Eng_Default.CommandPermsChanged("Rank by parameter",
                                            value.ToString());
                                    }
                                    else if (e.GetArg("Command").ToLower() == "regionaccount")
                                    {
                                        new SettingsRepo(new SettingsContext()).ToggleRegionAccount(value, e.Server.Id);
                                        returnstring = Eng_Default.CommandPermsChanged("Region by account",
                                            value.ToString());
                                    }
                                    else if (e.GetArg("Command").ToLower() == "regionparameter")
                                    {
                                        new SettingsRepo(new SettingsContext()).ToggleRegionParameter(value, e.Server.Id);
                                        returnstring = Eng_Default.CommandPermsChanged("Region by parameter",
                                            value.ToString());
                                    }
                                    else
                                    {
                                        returnstring = Eng_Default.PermsCommandNotFound();
                                    }
                                }
                                else
                                {
                                    returnstring = Eng_Default.NotAllowed();
                                }
                            }
                            else
                            {
                                returnstring = Eng_Default.InvalidBoolValue();
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
            private void OverrideSystem()
            {
                //Temporary name Override, needs a better name like CustomRole, just programming it now for the functionallity
                commands.CreateCommand("Override")
                    .Parameter("CommandType", ParameterType.Optional)
                    .Parameter("Role", ParameterType.Optional)
                    .Parameter("Parameter", ParameterType.Unparsed)
                    .Do(async (e) =>
                    {
                        string returnstring = "";
                        if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                        {
                            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                            if (new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id) == true)
                            {
                                if (e.GetArg("CommandType").ToLower() == "help" || e.GetArg("CommandType") == "?")
                                {
                                    //Gives all of the information about -Override and its overloads
                                    returnstring += "**With this command you can add custom ranks to your server.**" +
                                                    "\nUse the format: *-Override Add \"RoleName\" Parameter*" +
                                                    "\nYou can find out what parameters you can use by using one of the following commands:" +
                                                    "\n*-Rank List, -Region List, -Role List, -Mastery List*" +
                                                    "\n\nYou can also find out what overrides you have with -Override List." +
                                                    "\nThese overrides can be removed using *-Override remove <id>*.";
                                }
                                else if (e.GetArg("CommandType").ToLower() == "remove" &&
                                         e.GetArg("Parameter").ToLower() != "disable")
                                {
                                    try
                                    {
                                        settingsRepo.RemoveOverride(Convert.ToInt32(e.GetArg("Role")), e.Server.Id);
                                        returnstring = Eng_Default.OverrideRemoved();
                                    }
                                    catch
                                    {
                                        returnstring = Eng_Default.OverrideFailedToRemoved(e.GetArg("Role"));
                                    }

                                }
                                else if ((e.GetArg("CommandType").ToLower() == "remove" ||
                                          e.GetArg("CommandType").ToLower() == "delete") &&
                                         e.GetArg("Parameter").ToLower() == "disable")
                                {
                                    settingsRepo.RemoveRoleDisable(Convert.ToInt32(e.GetArg("Role")), e.Server.Id);
                                }
                                else if (e.GetArg("CommandType").ToLower() == "add")
                                {
                                    //Adds an override to the system
                                    if (e.GetArg("Parameter").ToLower() == "disable")
                                    {
                                        settingsRepo.AddRoleDisable(e.GetArg("Role"), e.Server.Id);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            ulong id = 0;
                                            try
                                            {
                                                id = e.Server.FindRoles(e.GetArg("Role"), false).First().Id;
                                            }
                                            catch
                                            {
                                                throw new Exception("Role not found");
                                            }
                                            if (id != 0)
                                            {
                                                if (e.GetArg("Parameter").IndexOf(" ") == 0)
                                                {
                                                    settingsRepo.AddOverride(
                                                        e.GetArg("Parameter").ToString().ToLower().Remove(0, 1), id
                                                        , e.Server.Id);
                                                }
                                                else if (e.GetArg("Parameter").IndexOf(" ") == (e.GetArg("Parameter").Length))
                                                {
                                                    settingsRepo.AddOverride(
                                                        e.GetArg("Parameter")
                                                            .ToString()
                                                            .ToLower()
                                                            .Remove(e.GetArg("Parameter").Length, 1), id
                                                        , e.Server.Id);
                                                }
                                                else
                                                {
                                                    settingsRepo.AddOverride(
                                                        e.GetArg("Parameter").ToString().ToLower(), id,
                                                        e.Server.Id);
                                                }

                                                returnstring = Eng_Default.OverrideAdded();
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            returnstring = ex.Message;
                                        }
                                        catch
                                        {
                                            returnstring = Eng_Default.OverrideFailedToAdd();
                                        }
                                    }
                                }
                                else if (e.GetArg("CommandType").ToLower() == "list")
                                {
                                    int entries = 0;
                                    returnstring = "```\nOverrides:";
                                    //Gives a list of all the overrides made by this server.
                                    foreach (string line in settingsRepo.GetAllOverridesInformation(e.Server.Id))
                                    {
                                        //await e.Channel.SendMessage(line.Substring(line.IndexOf("role:") + 5, line.Length - line.IndexOf("role:") - 6));
                                        ulong id =
                                            Convert.ToUInt64(line.Substring(line.IndexOf("role:") + 5,
                                                line.Length - line.IndexOf("role:") - 5));
                                        var role = e.Server.GetRole(id);
                                        try
                                        {
                                            returnstring += "\n" + line.Substring(0, line.IndexOf("role:") + 5) + " " +
                                                            role.Name;
                                        }
                                        catch
                                        {
                                            new SettingsRepo(new SettingsContext()).RemoveOverride(
                                                Convert.ToInt32(line.Split(' ')[1]), e.Server.Id);
                                        }
                                        entries++;
                                    }
                                    returnstring += "\nDisables:";
                                    foreach (string line in settingsRepo.GetDisabledRoles(e.Server.Id))
                                    {
                                        returnstring += "\n" + line;
                                        entries++;

                                    }
                                    returnstring += "\n```";
                                    if (entries == 0)
                                    {
                                        returnstring = Eng_Default.NoOverrides();
                                    }
                                }
                            }
                            else
                            {
                                returnstring = Eng_Default.NotAllowed();
                            }
                        }
                        else
                        {
                            returnstring = Eng_Default.ServerIsNotVerified();
                        }
                        
                        await e.Channel.SendMessage(returnstring);
                    });
            }
            #endregion ServerManagement

            #region AccountManagement

            private void ClaimAccount()
            {
                commands.CreateCommand("ClaimAccount")
                    .Parameter("Region", ParameterType.Required)
                    .Parameter("Summoner", ParameterType.Unparsed)
                    .Do(async (e) =>
                    {
                        string returnmessage = "An error happened.";
                        if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                        {
                            Region region = LeagueAndDatabase.GetRegionFromString(e.GetArg("Region"));
                            string summonername = e.GetArg("Summoner");
                            SummonerRepo sumRepo = new SummonerRepo(new SummonerContext());
                            UserRepo userRepo = new UserRepo(new UserContext());
                            int riotid = Convert.ToInt32(new SummonerAPI().GetSummonerId(summonername, region));
                            string token = RandomStringGenerator();
                            if (
                                sumRepo.IsSummonerInSystem(riotid
                                ) == false
                            )
                            {
                                try
                                {
                                    userRepo.GetUserIdByDiscord((e.User.Id));
                                }
                                catch
                                {
                                    userRepo.AddUser(Convert.ToInt64(e.User.Id));
                                }
                                sumRepo.AddSummoner(userRepo.GetUserIdByDiscord((e.User.Id)), riotid,
                                    new RegionContext().GetRegionId(region), token);
                                returnmessage =
                                    Eng_Default.RenameMasteryPage(
                                        sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), riotid).ToString());
                            }
                            else
                            {
                                returnmessage =
                                    Eng_Default.RenameMasteryPageLong(
                                        sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), riotid));
                                string token2 = sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), riotid);
                                foreach (var page in new SummonerAPI().GetSummonerMasteryPages(summonername, region))
                                {
                                    if (page.Name.ToLower() == token2.ToLower())
                                    {
                                        sumRepo.VerifySummoner(userRepo.GetUserByDiscord((e.User.Id)), riotid);
                                        returnmessage = Eng_Default.AccountVerified();
                                    }
                                }
                            }
                        }
                        else
                        {
                            returnmessage = Eng_Default.ServerIsNotVerified();
                        }
                        
                        await e.Channel.SendMessage(returnmessage);
                    });
            }

            private void GetRank()
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
                                OverrideDeletion(e.Server);
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
                                                "Please register your account by using -ClaimAccount *Region SummonerName*";
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
                                        else
                                        {
                                            returnstring = Eng_Default.ServerDoesNotAllow();
                                        }
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

            private void GetRegion()
            {
                commands.CreateCommand("region")
                    .Parameter("region", ParameterType.Unparsed)
                    .Do(async (e) =>
                    {
                        try {OverrideDeletion(e.Server); } catch { }
                        string returnstring = "error";
                        if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                        {
                            string command = "";
                            try
                            {
                                command = e.GetArg("region").Substring(0, e.GetArg("region").IndexOf(" ")).ToLower();
                            }
                            catch { }

                            SettingsRepo settingsRepo = (new SettingsRepo(new SettingsContext()));
                            if (e.GetArg("region").ToLower() == "help" || e.GetArg("region") == "?")
                            {
                                if (settingsRepo.RegionByAccount(e.Server.Id) == true ||
                                    settingsRepo.RegionByParameter(e.Server.Id) == true)
                                {
                                    returnstring = "You can use the -Region command to assign a region role to yourself.";
                                    if (settingsRepo.RegionByAccount(e.Server.Id) == true)
                                    {
                                        returnstring +=
                                            "\nYou can use *-Region* to get your region based on your bound League of Legends account.";
                                    }
                                    if (settingsRepo.RegionByParameter(e.Server.Id) == true)
                                    {
                                        returnstring +=
                                            "\nYou can use *-Region <League Region>* to assign a region to yourself.\nUse *-Region list* to see all the regions on this server.";
                                    }
                                }

                                else
                                {
                                    returnstring = Eng_Default.ServerDoesNotAllow();
                                }
                            }
                            else if (command == "remove" || command == "delete")
                            {

                                foreach (string region in new RegionRepo(new RegionContext()).GetAllRegions())
                                {
                                    if (region.ToLower() == e.GetArg("region").Split(' ').Last().ToLower())
                                    {
                                        try
                                        {
                                            ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                            await e.User.RemoveRoles(e.Server.GetRole(id));
                                            returnstring = Eng_Default.RoleHasBeenRemoved(region);
                                        }
                                        catch
                                        {
                                            await e.User.RemoveRoles(e.Server.FindRoles(region.ToLower(), false).First());
                                            returnstring = Eng_Default.RoleHasBeenRemoved(region);
                                        }
                                    }
                                }
                                try
                                {
                                    foreach (string role in settingsRepo.GetAllOverrides(e.Server.Id))
                                    {
                                        var replacement = e.Server.GetRole(Convert.ToUInt64(role.Split(':').Last()));
                                        if (e.GetArg("region").Substring(e.GetArg("region").IndexOf(" ") + 1, e.GetArg("region").Length - e.GetArg("region").IndexOf(" ") - 1).ToLower() == replacement.Name.ToLower())
                                        {
                                            await e.User.RemoveRoles(replacement);
                                            returnstring = Eng_Default.RoleHasBeenRemoved(role);
                                        }
                                    }
                                }
                                catch { }

                            }
                            else if (e.GetArg("region").ToLower() == "list")
                            {
                                if (settingsRepo.RegionByParameter(e.Server.Id) == true ||
                                    settingsRepo.RegionByAccount(e.Server.Id) == true)
                                {
                                    returnstring = "Assignable regions on this server:\n```";
                                    List<string> overrides = new List<string>();
                                    try
                                    {
                                        overrides = settingsRepo.GetAllOverrides(e.Server.Id);
                                    }
                                    catch { }
                                    foreach (string region in new RegionRepo(new RegionContext()).GetAllRegions())
                                    {
                                        bool found = false;
                                        if (overrides != null)
                                        {
                                            foreach (string over in overrides)
                                            {
                                                if (region.ToLower() == over.Substring(0, over.IndexOf(":")).ToLower())
                                                {
                                                    returnstring += "\n- " + e.Server.GetRole(Convert.ToUInt64(over.Substring(over.IndexOf(":") + 1, over.Length - over.IndexOf(":") - 1))).Name;
                                                    found = true;
                                                }
                                            }
                                        }
                                        if (found == false)
                                        {
                                            returnstring += "\n- " + region;
                                        }
                                    }
                                    returnstring += "```";
                                }
                                else
                                {
                                    returnstring = Eng_Default.ServerDoesNotAllow();
                                }
                            }
                            else
                            {
                                if (settingsRepo.RegionByAccount(e.Server.Id) == true && e.GetArg("region") == "")
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
                                                    await e.User.AddRoles(
                                                        e.Server.GetRole(settingsRepo.GetOverride(region.ToLower(), e.Server.Id)));
                                                    returnstring = Eng_Default.RoleHasBeenGiven(region);
                                                }
                                                catch
                                                {
                                                    await e.User.AddRoles(e.Server.FindRoles(region, false).First());
                                                    returnstring = Eng_Default.RoleHasBeenGiven(region);
                                                }
                                            }

                                        }
                                    }
                                }
                                else if (e.GetArg("region") == "")
                                {
                                    returnstring = Eng_Default.ServerDoesNotAllow();
                                }
                                else if (settingsRepo.RegionByParameter(e.Server.Id) == true)
                                {
                                    bool found = false;
                                    foreach (string region in new RegionRepo(new RegionContext()).GetAllRegions())
                                    {
                                        if (e.GetArg("region").ToLower() == region.ToLower())
                                        {
                                            try
                                            {
                                                Discord.Role r =
                                                    e.Server.GetRole(settingsRepo.GetOverride(region.ToLower(), e.Server.Id));
                                                if (settingsRepo.IsRoleDisabled(r.Name.ToLower(), e.Server.Id))
                                                {
                                                    returnstring = Eng_Default.RoleHasBeenDisabled();
                                                }
                                                else
                                                {
                                                    await e.User.AddRoles();
                                                    returnstring = Eng_Default.RoleHasBeenGiven(r.Name);
                                                }

                                                found = true;
                                            }
                                            catch
                                            {
                                                Discord.Role r = e.Server.FindRoles(region, false).First();
                                                if (settingsRepo.IsRoleDisabled(r.Name.ToLower(), e.Server.Id))
                                                {
                                                    returnstring = Eng_Default.RoleHasBeenDisabled();
                                                }
                                                else
                                                {
                                                    await e.User.AddRoles();
                                                    returnstring = Eng_Default.RoleHasBeenGiven(r.Name);
                                                }
                                                found = true;
                                            }
                                        }
                                    }
                                    foreach (string role in settingsRepo.GetAllOverrides(e.Server.Id))
                                    {
                                        var replacement = e.Server.GetRole(Convert.ToUInt64(role.Substring(role.IndexOf(":") + 1, role.Length - role.IndexOf(":") - 1)));
                                        if (e.GetArg("region").ToLower() == replacement.Name.ToLower())
                                        {
                                            await e.User.AddRoles(replacement);
                                            returnstring = Eng_Default.RoleHasBeenGiven(role);
                                            found = true;
                                        }
                                    }
                                    if (found == false)
                                    {
                                        returnstring = Eng_Default.RoleNotFound(e.GetArg("region"));
                                    }
                                }
                                else
                                {
                                    returnstring = Eng_Default.ServerDoesNotAllow();
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

            private void OverrideDeletion(Discord.Server server)
            {
                string thisshouldntbeneededbutiguessitis = "";
                SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                foreach (string line in settingsRepo.GetAllOverrides(server.Id))
                {
                    ulong id = Convert.ToUInt64(line.Substring(line.IndexOf("role:") + 5, line.Length - line.IndexOf("role:") - 5));
                    var role = server.GetRole(id);
                    try
                    {
                        thisshouldntbeneededbutiguessitis = role.Name;
                    }
                    catch
                    {
                        new SettingsRepo(new SettingsContext()).RemoveOverride(Convert.ToInt32(line.Split(' ')[1]), server.Id);
                    }
                }
            }
            #endregion

            #region  logging

            private  void DMBort(string message)
            {
                BotUser.GetServer(291643233682063370).FindUsers("Bort", true).First().SendMessage(message);
            }

            private void AdminLog(string message)
            {
                BotUser.GetServer(291643233682063370).GetChannel(291643340678627328).SendMessage(message);
            }

            #endregion


            private string RandomStringGenerator()
            {
                Guid g = Guid.NewGuid();
                string guidString = Convert.ToBase64String(g.ToByteArray());
                guidString = guidString.Replace("=", "");
                guidString = guidString.Replace("+", "");
                guidString = guidString.Replace("/", "");
                guidString = guidString.Substring(0, 10);
                return guidString;
            }
        }
    }
}