using System;
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

namespace AtlasBot.Modules.Region
{
    public class RegionCommands
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public RegionCommands(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public void GetRegion()
        {
            commands.CreateCommand("region")
                .Parameter("region", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    try { new RoleManagementTrigger(BotUser, commands).OverrideDeletion(e.Server); } catch { }
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
                                        await e.User.RemoveRoles(e.Server.FindRoles(region.ToLower(), false).First());
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
                            else if (e.GetArg("region") == "" && settingsRepo.RegionByAccount(e.Server.Id) == false)
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
                                                await e.User.AddRoles(r);
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
                                                await e.User.AddRoles(r);
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
    }
}
