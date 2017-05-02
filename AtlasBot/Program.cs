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
                        await e.Channel.SendMessage("Server has been verified!");
                    });
            }

            private void CheckForNewServer()
            {
                BotUser.Ready += async (s, u) =>
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
                    new SettingsRepo(new SettingsContext()).CreateSettings(serverid);
                    Console.WriteLine(servername + " has added AtlasBot to their server");
                    AdminLog(servername + " has added the bot. Owner: " + server.Owner.ToString());
                    DMBort(servername + ": " + server.Owner.ToString() + " Key: " + key);
                    server.Owner.SendMessage(
                        "Thank you for adding AtlasBot to your server!\nFor safety reasons we need verification from Bort that this is allowed.\nPlease use the command *-verify <key>* to verify your server!\nBort will contact you soon with your key.");

            }
            #endregion ServerJoining

            #region ServerManagement
            private void InviteLink()
            {
                commands.CreateCommand("SetInvite")
                    .Parameter("InviteLink", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        new ServerRepo(new ServerContext()).AddInviteLink(e.User.Id, e.Server.Id, e.GetArg("InviteLink"));
                        await e.Channel.SendMessage("Invite link has been set!");
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
                        if (new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id) == true)
                        {
                            if (e.GetArg("Type").ToLower() == "rank")
                            {
                                CommandType result;
                                CommandType.TryParse(e.GetArg("CommandType"), out result);
                                new SettingsRepo(new SettingsContext()).SetRankType(result, e.Server.Id);
                                returnstring = "Command type changed to " + e.GetArg("CommandType");
                            }
                        }
                        else
                        {
                            returnstring = "You are not permitted to do this.";
                        }
                        await e.Channel.SendMessage(returnstring);
                    });
            }

            private void ChangeCommandAllowed()
            {
                commands.CreateCommand("Command")
                    .Parameter("Command", ParameterType.Required)
                    .Parameter("Value", ParameterType.Required)
                    .Do(async (e)=>
                {
                    string returnstring = "error";
                    if (new ServerRepo(new ServerContext()).IsAdmin(e.User.Id, e.Server.Id) == true)
                    {
                        if (e.GetArg("Command").ToLower() == "rank")
                        {
                            bool value;
                            bool.TryParse(e.GetArg("Value"), out value);
                            new SettingsRepo(new SettingsContext()).ToggleAccountRank(value, e.Server.Id);
                            returnstring = "Rank has been changed to " + value.ToString();
                        }
                    }
                    else
                    {
                        returnstring = "You don't have permission to do this.";
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
                            else if (e.GetArg("CommandType").ToLower() == "remove")
                            {
                                try
                                {
                                    settingsRepo.RemoveOverride(Convert.ToInt32(e.GetArg("Role")), e.Server.Id);
                                    returnstring = "Override removed successfully";
                                }
                                catch
                                {
                                    returnstring = "Could not remove override with id " + e.GetArg("Role");
                                }
                                
                            }
                            else if (e.GetArg("CommandType").ToLower() == "add")
                            {
                                //Adds an override to the system
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
                                            settingsRepo.AddOverride(e.GetArg("Parameter").ToString().ToLower(), id,
                                                e.Server.Id);
                                        }

                                        returnstring = "Override has been saved";
                                    }

                                }
                                catch (Exception ex)
                                {
                                    returnstring = ex.Message;
                                }
                                catch
                                {
                                    returnstring = "Override has failed to save";
                                }
                                
                            }
                            else if (e.GetArg("CommandType").ToLower() == "list")
                            {
                                int entries = 0;
                                returnstring = "```";
                                //Gives a list of all the overrides made by this server.
                                foreach (string line in settingsRepo.GetAllOverridesInformation(e.Server.Id))
                                {
                                    //await e.Channel.SendMessage(line.Substring(line.IndexOf("role:") + 5, line.Length - line.IndexOf("role:") - 6));
                                    ulong id = Convert.ToUInt64(line.Substring(line.IndexOf("role:") + 5, line.Length - line.IndexOf("role:") - 5));
                                    var role = e.Server.GetRole(id);
                                    try
                                    {
                                        returnstring += "\n" + line.Substring(0, line.IndexOf("role:") + 5) + " " +
                                                        role.Name;
                                    }
                                    catch
                                    {
                                        new SettingsRepo(new SettingsContext()).RemoveOverride(Convert.ToInt32(line.Split(' ')[1]), e.Server.Id);
                                    }
                                    entries++;
                                }
                                returnstring += "\n```";
                                if (entries == 0)
                                {
                                    returnstring = "There are no overrides for this server";
                                }
                            }
                        }
                        else
                        {
                            returnstring = "You are not permitted to do this.";
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
                            returnmessage = "Please rename one of your masterypages to: " + sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), riotid).ToString();
                        }
                        else
                        {
                            returnmessage = "Please rename one of your masterypages to: " + sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), riotid) +
                                            "\nIt may take a little for the RiotAPI to update, please stay patient!";
                            string token2 = sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), riotid);
                            foreach (var page in new SummonerAPI().GetSummonerMasteryPages(summonername, region))
                            {
                            if (page.Name.ToLower() == token2.ToLower())
                            {
                                sumRepo.VerifySummoner(userRepo.GetUserByDiscord((e.User.Id)), riotid);
                                returnmessage = "Your account has been verified";
                            }
                        }
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
                        string returnstring = "error"; SettingsRepo settingsRepo = (new SettingsRepo(new SettingsContext()));
                        if (e.GetArg("rank").Split(' ').First() == "delete" ||
                            e.GetArg("rank").Split(' ').First() == "remove")
                        {
                            foreach (string region in Ranks.BasicRanks())
                            {
                                if (region.ToLower() == e.GetArg("rank").Substring(e.GetArg("rank").IndexOf(" ") + 1, e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1).ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id), e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = "Your role has been removed.";
                                    }
                                    catch { }
                                }
                            }
                            foreach (string region in Ranks.QueueRanks())
                            {
                                if (region.ToLower() == e.GetArg("rank").Substring(e.GetArg("rank").IndexOf(" ") + 1, e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1).ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id), e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = "Your role has been removed.";
                                    }
                                    catch { }
                                }
                            }
                            foreach (string region in Ranks.DivisionRanks())
                            {
                                if (region.ToLower() == e.GetArg("rank").Substring(e.GetArg("rank").IndexOf(" ") + 1, e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1).ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id), e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = "Your role has been removed.";
                                    }
                                    catch { }
                                }
                            }
                            try
                            {
                                foreach (string role in settingsRepo.GetAllOverrides(e.Server.Id))
                                {
                                    var replacement = e.Server.GetRole(Convert.ToUInt64(role.Split(':').Last()));
                                    if (e.GetArg("rank").Substring(e.GetArg("rank").IndexOf(" ") + 1, e.GetArg("rank").Length - e.GetArg("rank").IndexOf(" ") - 1).ToLower() == replacement.Name.ToLower())
                                    {
                                        await e.User.RemoveRoles(replacement);
                                        returnstring = "Your role has been removed";
                                    }
                                }
                            }
                            catch { }
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
                                returnstring = "The server does not allow this feature";
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
                                returnstring += "\nYou can use *-Rank <League rank>* to get a role based on your input";
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
                                            string rank = new RankAPI().GetRankingSimple(summoner, Queue.RankedSolo5x5);
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
                                            returnstring = "Your rank has been granted.";
                                        }
                                        else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Division)
                                        {
                                            string rank =
                                                new RankAPI().GetRankingHarder(summoner, Queue.RankedSolo5x5).ToLower();
                                            try
                                            {
                                                await e.User.AddRoles(
                                                    e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id)));
                                            }
                                            catch
                                            {
                                                await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                            }

                                            returnstring = "Your rank has been granted.";
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
                                            returnstring = "Your ranks have been granted.";
                                        }
                                    }
                                    else
                                    {
                                        returnstring = "The server doesn't allow this action.";
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
                                                await e.User.AddRoles(
                                                    e.Server.GetRole(settingsRepo.GetOverride(rank, e.Server.Id)));
                                            }
                                            catch
                                            {
                                                await e.User.AddRoles(e.Server.FindRoles(rank, false).First());
                                            }
                                            returnstring = "Your rank has been granted.";
                                            found = true;
                                        }
                                    }
                                    if (found == false)
                                    {
                                        returnstring = "Didn't find the rank called " + e.GetArg("rank");
                                    }
                                }
                                else
                                {
                                    returnstring = "The server doesn't allow this action.";
                                }
                            }
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
                        string returnstring = "error";
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
                                returnstring = "This action is not permitted on this server.";
                            }
                        }
                        else if (command == "remove" ||command == "delete")
                        {
                            
                            foreach (string region in new RegionRepo(new RegionContext()).GetAllRegions())
                            {
                                if (region.ToLower() == e.GetArg("region").Split(' ').Last().ToLower())
                                {
                                    try
                                    {
                                        ulong id = settingsRepo.GetOverride(region.ToLower(), e.Server.Id);
                                        await e.User.RemoveRoles(e.Server.GetRole(id));
                                        returnstring = "Your role has been removed.";
                                    }
                                    catch
                                    {
                                        await e.User.RemoveRoles(e.Server.FindRoles(region.ToLower(), false).First());
                                        returnstring = "Your role has been removed.";
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
                                        returnstring = "Your role has been removed";
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
                                } catch { }
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
                                returnstring = "This action is not allowed on this server";
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
                                    returnstring =
                                        "Please register your account by using -ClaimAccount *Region SummonerName*";
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
                                                returnstring = "Your role has been given";
                                            }
                                            catch
                                            {
                                                await e.User.AddRoles(e.Server.FindRoles(region, false).First());
                                                returnstring = "Your role has been given";
                                            }
                                        }

                                    }
                                }
                            }
                            else if (e.GetArg("region") == "")
                            {
                                returnstring = "This action is not allowed on this server.";
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
                                            await e.User.AddRoles(
                                                e.Server.GetRole(settingsRepo.GetOverride(region.ToLower(), e.Server.Id)));
                                            returnstring = "Your role has been given";
                                            found = true;
                                        }
                                        catch
                                        {
                                            await e.User.AddRoles(e.Server.FindRoles(region, false).First());
                                            returnstring = "Your role has been given";
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
                                        returnstring = "Your role has been given";
                                        found = true;
                                    }
                                }
                                if (found == false)
                                {
                                    returnstring = "No role found called " + e.GetArg("region");
                                }
                            }
                            else
                            {
                                returnstring = "This action is not allowed on this server.";
                            }
                        }
                        await e.Channel.SendMessage(returnstring);
                    });
            }
            #endregion
            private void DMBort(string message)
            {
                BotUser.GetServer(291643233682063370).FindUsers("Bort", true).First().SendMessage(message);
            }

            private void AdminLog(string message)
            {
                BotUser.GetServer(291643233682063370).GetChannel(291643340678627328).SendMessage(message);
            }

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

//championApi.Get5MainChampions(sumApi.GetSummoner(username, Region.euw), Region.euw).ForEach(champ =>
//{
//    Console.WriteLine(champ.Name + ": " + champ.Count);
//});
//SummonerAPI sumApi = new SummonerAPI();
//ChampionAPI championApi = new ChampionAPI();
//MasteryAPI masteryApi = new MasteryAPI();
//Console.WriteLine("Type a username");
//            string username = Console.ReadLine();
//Summoner sum = sumApi.GetSummoner(username, Region.euw);
//Console.WriteLine(masteryApi.GetPoints(sum, championApi.GetChampion("Thresh")));
//            Console.ReadLine();


//Guid g = Guid.NewGuid();
//string GuidString = Convert.ToBase64String(g.ToByteArray());
//GuidString = GuidString.Replace("=", "");
//            GuidString = GuidString.Replace("+", "");
//            GuidString = GuidString.Replace("/", "");
//            GuidString = GuidString.Substring(0, 10);
//            Console.WriteLine(GuidString);
//            Console.ReadLine();