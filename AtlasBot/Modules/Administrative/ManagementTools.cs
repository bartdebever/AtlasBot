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

namespace AtlasBot.Modules.Administrative
{
    public class ManagementTools
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public ManagementTools(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public void Admin()
        {
            commands.CreateCommand("Admin")
                .Parameter("CommandType")
                .Parameter("User", ParameterType.Optional)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    if (e.User.Id == e.Server.Owner.Id && e.GetArg("CommandType").ToLower() == "add")
                    {
                        new ServerRepo(new ServerContext()).AddAdmin(e.Message.MentionedUsers.First().Id, e.Server.Id);
                        returnstring = "Admin has been added";
                    }
                    else if (e.User.Id == e.Server.Owner.Id && e.GetArg("CommandType").ToLower() == "list")
                    {
                        returnstring = "Admins on " + e.Server.Name + ":```";
                        foreach (string admin in new ServerRepo(new ServerContext()).ListAdmins(e.Server.Id))
                        {
                            returnstring += "\n-" + e.Server.GetUser(Convert.ToUInt64(admin)).Name;
                        }
                        returnstring += "```";
                    }
                    else if (e.User.Id == e.Server.Owner.Id && e.GetArg("CommandType").ToLower() == "remove")
                    {
                        new ServerRepo(new ServerContext()).RemoveAdmin(e.Message.MentionedUsers.First().Id, e.Server.Id);
                        returnstring = e.Message.MentionedUsers.First().Name + " has been removed from admin.";
                    }
                    else
                    {
                        returnstring = Eng_Default.NotAllowed();
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }

        public void ChangeType()
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
                            else if (e.GetArg("Type").ToLower() == "role")
                            {
                                CommandType result;
                                try
                                {
                                    CommandType.TryParse(e.GetArg("CommandType"), out result);
                                    new SettingsRepo(new SettingsContext()).SetRoleType(result, e.Server.Id);
                                    returnstring = Eng_Default.CommandTypeChange("rank", e.GetArg("CommandType"));
                                }
                                catch
                                {
                                    returnstring = "Correct commandtypes for Role are:" +
                                                   "\n**Basic**: Just simple Top, Jungle, Mid, ADC, Support" +
                                                   "\n**Main**: Makes it Top-Main, Jungle-Main, etc" +
                                                   "\n**Mains**: Makes it Top-Mains, Jungle-Mains, etc";
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

                    }

                    await e.Channel.SendMessage(returnstring);
                });
        }

        public void ChangeCommandAllowed()
        {
            commands.CreateCommand("Command")
                .Parameter("Command", ParameterType.Required)
                .Parameter("Value", ParameterType.Optional)
                .Do(async (e) =>
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
                                    "\n- Rankparameter: Allow the user to type -rank <rank> to get a rank role assinged." +
                                    "\n- Roleaccount: Allow the user to type -role and get a role assigned that fits their main role in League of Legends." +
                                    "\n- Roleparameter: Allow the user to type -role <role> to get that role assigned." +
                                    "\n- MasteryAccount: Allow the user to type -mastery and get their amount of points assigned." +
                                    "\n\nPlease use the format -Command <Command> <Value>, example: *-Command rankaccount true*.";
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
                                    else if (e.GetArg("Command").ToLower() == "roleaccount")
                                    {
                                        new SettingsRepo(new SettingsContext()).ChangeRoleAccount(value, e.Server.Id);
                                        returnstring = Eng_Default.CommandPermsChanged("Role by account",
                                                value.ToString());
                                    }
                                    else if (e.GetArg("Command").ToLower() == "roleparameter")
                                    {
                                        new SettingsRepo(new SettingsContext()).ChangeRoleParameter(value, e.Server.Id);
                                        returnstring = Eng_Default.CommandPermsChanged("Role by parameter",
                                                value.ToString());
                                    }
                                    else if (e.GetArg("Command").ToLower() == "masteryaccount")
                                    {
                                        new SettingsRepo(new SettingsContext()).ChangeMasteryAccount(value, e.Server.Id);
                                        returnstring =
                                                Eng_Default.CommandPermsChanged("Mastery by account", value.ToString()) +
                                                "\nDon't forget to configure this by using -ConfigMastery!";
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
        public void OverrideSystem()
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
                                                "\n*-Rank List, -Region List, -Role List, -Mastery List.*" +
                                                "\n\nYou can also find out what overrides you have with -Override List." +
                                                "\nThese overrides can be removed using *-Override remove <id>*." +
                                                "\n\nYou can also disable roles, these will not be getable by parameter (-rank master)" +
                                                "\nUse *-Override add <Role> disable* to disable a role." +
                                                "\nYou can see disables in the override list using *-Override list*." +
                                                "\nYou can remove disables by using *-Override remove <id> disable*.";
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
                                try
                                {
                                    settingsRepo.RemoveRoleDisable(Convert.ToInt32(e.GetArg("Role")), e.Server.Id);
                                    returnstring = "Disable has been removed.";
                                }
                                catch
                                {
                                    returnstring = "Failed to remove disable.";
                                }

                            }
                            else if (e.GetArg("CommandType").ToLower() == "add")
                            {
                                    //Adds an override to the system
                                    if (e.GetArg("Parameter").ToLower() == "disable")
                                {
                                    try
                                    {
                                        settingsRepo.AddRoleDisable(e.GetArg("Role"), e.Server.Id);
                                        returnstring = "Successfully added disable.";
                                    }
                                    catch
                                    {
                                        returnstring = "Failed to add disable.";
                                    }

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


        public void AdminMastery()
        {
            commands.CreateCommand("ConfigMastery")
                .Parameter("CommandType")
                .Parameter("Parameter", ParameterType.Optional)
                .Parameter("Points", ParameterType.Optional)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    if (serverRepo.IsServerVerified(e.Server.Id) && serverRepo.IsAdmin(e.User.Id, e.Server.Id))
                    {
                        if (e.GetArg("CommandType") == "?" || e.GetArg("CommandType").ToLower() == "help")
                        {
                            returnstring =
                                "**This command can be used to configure the Mastery Point system on your server.**" +
                                "\nUse *-ConfigMastery champion <champion>* to set a champion for your server:" +
                                "\nExample: -ConfigMastery champion Thresh" +
                                "\n\nUse *-ConfigMastery list* to get a list of all the roles you have set up." +
                                "\n\nUse *-ConfigMastery add <RoleName> <Amount>* to add a rank to the system:" +
                                "\nExample: -ConfigMastery add \"1 Million\" 1000000" +
                                "\n\nUse *-ConfigMastery remove <Points>* to remove a milestone rank:" +
                                "\nExample: -ConfigMastery remove 1000000";
                        }
                        else if (e.GetArg("CommandType").ToLower() == "champion")
                        {
                            try
                            {
                                settingsRepo.SetChampionId(e.Server.Id, new ChampionAPI().GetChampionId(e.GetArg("Parameter").ToString()));
                                returnstring = "Champion set to " + e.GetArg("Parameter");
                            }
                            catch
                            {
                                returnstring = "Did not find champion called " + e.GetArg("Parameter");
                            }

                        }
                        else if (e.GetArg("CommandType").ToLower() == "list")
                        {
                            try
                            {
                                returnstring = "Server looks at mastery points for " +
                                               new ChampionAPI().GetChampionName(
                                                   settingsRepo.GetChampionId(e.Server.Id));
                                returnstring += "\nRoles for this server: ```";
                                foreach (string line in settingsRepo.GetAllMasteryRoles(e.Server.Id))
                                {
                                    returnstring += "\n" + line.Split(':').First() + " points uses role: " +
                                                    e.Server.GetRole(Convert.ToUInt64(line.Split(':').Last())).Name;
                                }
                                returnstring += "\n```";
                            }
                            catch
                            {
                                returnstring = "No champion or role found, please set this up first!";
                            }

                        }
                        else if (e.GetArg("CommandType").ToLower() == "add")
                        {
                            try
                            {
                                Discord.Role r = e.Server.FindRoles(e.GetArg("Parameter"), false).First();
                                settingsRepo.SetRoleByPoints(r.Id, e.Server.Id, Convert.ToInt32(e.GetArg("Points")));
                                returnstring = "Added to the list!";
                            }
                            catch
                            {
                                returnstring = "Failed to add, role not found.";
                            }

                        }
                        else if (e.GetArg("CommandType").ToLower() == "remove")
                        {
                            try
                            {
                                settingsRepo.RemoveRoleByPoints(e.Server.Id, Convert.ToInt32(e.GetArg("Parameter")));
                                returnstring = "Removed the role with the points " + e.GetArg("Parameter");
                            }
                            catch
                            {
                                returnstring = "Failed to remove the role with points " + e.GetArg("Parameter");
                            }
                        }
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
