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
using Discord.Commands.Permissions.Visibility;
using Languages;
using RiotLibary.Roles;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Roles
{
    public class RoleCommand
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public RoleCommand(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public void GetRole()
        {
            commands.CreateCommand("Role")
                .Do(async (e) =>
                {
                    string returnstring = "";
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        if (settingsRepo.RoleByAccount(e.Server.Id))
                        {
                            List<string> filter = new List<string>();
                            if (settingsRepo.RoleCommandType(e.Server.Id) == CommandType.Basic)
                            {
                                filter = DataLibary.Models.Roles.NormalRoles();
                            }
                            else if (settingsRepo.RoleCommandType(e.Server.Id) == CommandType.Main)
                            {
                                filter = DataLibary.Models.Roles.MainRoles();
                            }
                            else if (settingsRepo.RoleCommandType(e.Server.Id) == CommandType.Mains)
                            {
                                filter = DataLibary.Models.Roles.MainsRoles();
                            }

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
                                string mainrole = new RoleAPI().GetRole(summoner);
                                foreach (string role in filter)
                                {
                                    if (role.Contains(mainrole))
                                    {
                                        try
                                        {
                                            ulong id = settingsRepo.GetOverride(role.ToLower(), e.Server.Id);
                                            await e.User.AddRoles(e.Server.GetRole(id));
                                            returnstring = Eng_Default.RoleHasBeenGiven(role);
                                        }
                                        catch
                                        {
                                            await e.User.AddRoles(e.Server.FindRoles(role, false).First());
                                            returnstring = Eng_Default.RoleHasBeenGiven(role);
                                        }
                                    }
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
        public void GetRoleParameter()
        {
            commands.CreateCommand("Role")
                .Parameter("Role", ParameterType.Required)
                .Parameter("Optional", ParameterType.Optional)
                .Do(async (e) =>
                {

                    string returnstring = "";
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                        List<string> filter = new List<string>();
                        if (e.GetArg("Role").ToLower() == "?" || e.GetArg("Role").ToLower() == "help")
                        {
                            if (settingsRepo.RoleByParameter(e.Server.Id) || settingsRepo.RoleByAccount(e.Server.Id))
                            {
                                returnstring = "You can use -Role to assign a role based on a League of Legends role.";
                                if (settingsRepo.RoleByAccount(e.Server.Id))
                                {
                                    returnstring +=
                                        "\nYou can use *-Role* to get your role automatically assigned based on your linked League of Legends account.";
                                }
                                if (settingsRepo.RoleByParameter(e.Server.Id))
                                {
                                    returnstring += "\nYou can use *-Role <Role>* to assgined yourself a role." +
                                                    "\nFor all assignable roles use -Role list.";

                                }
                            }
                        }

                        if (settingsRepo.RoleByParameter(e.Server.Id))
                        {
                            if (settingsRepo.RoleCommandType(e.Server.Id) == CommandType.Basic)
                            {
                                filter = DataLibary.Models.Roles.NormalRoles();
                            }
                            else if (settingsRepo.RoleCommandType(e.Server.Id) == CommandType.Main)
                            {
                                filter = DataLibary.Models.Roles.MainRoles();
                            }
                            else if (settingsRepo.RoleCommandType(e.Server.Id) == CommandType.Mains)
                            {
                                filter = DataLibary.Models.Roles.MainsRoles();
                            }


                            if (e.GetArg("Role").ToLower() == "list")
                            {
                                returnstring = "These roles are getable on the server:```";
                                foreach (string role in filter)
                                {
                                    returnstring += "\n- " + role;
                                }
                                returnstring += "```";
                            }
                            else if (e.GetArg("Role").ToLower() == "remove")
                            {
                                foreach (string role in filter)
                                {
                                    if (role.ToLower().Contains(e.GetArg("Optional").ToLower()))
                                    {
                                        try
                                        {
                                            await e.User.RemoveRoles(
                                                    e.Server.GetRole(settingsRepo.GetOverride(role.ToLower(),
                                                        e.Server.Id)));
                                            await e.User.RemoveRoles(e.Server.FindRoles(role, false).First());
                                            returnstring = Eng_Default.RoleHasBeenRemoved(role);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (string role in filter)
                                {
                                    if (role.ToLower().Contains(e.GetArg("Role").ToLower()))
                                    {
                                        try
                                        {
                                            ulong id = settingsRepo.GetOverride(role.ToLower(), e.Server.Id);
                                            await e.User.AddRoles(e.Server.GetRole(id));
                                            returnstring = Eng_Default.RoleHasBeenGiven(role);
                                        }
                                        catch
                                        {
                                            await e.User.AddRoles(e.Server.FindRoles(role, false).First());
                                            returnstring = Eng_Default.RoleHasBeenGiven(role);
                                        }
                                    }
                                }
                            }
                        }
                        else if (e.GetArg("Role").ToLower() != "?" && e.GetArg("Role").ToLower() != "help")
                        {
                            returnstring = Eng_Default.ServerDoesNotAllow();
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
