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

namespace AtlasBot.Modules.Roles
{
    public class Universal_Role
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public Universal_Role(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public Discord.Role RoleSearch(Discord.Server server, string parameter)
        {
            Discord.Role r = null;
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            if (settingsRepo.RankByParameter(server.Id) == true)
            {
                List<string> filter = new List<string>();
                if (settingsRepo.RankCommandType(server.Id) == CommandType.Basic)
                {
                    filter = Ranks.BasicRanks();
                }

                else if (settingsRepo.RankCommandType(server.Id) == CommandType.PerQueue)
                {
                    filter = Ranks.QueueRanks();
                }
                foreach (string rank in filter)
                {
                    if (parameter.ToLower() == rank.ToLower())
                    {
                        try
                        {
                            
                             return server.GetRole(settingsRepo.GetOverride(rank, server.Id));
                            

                        }
                        catch
                        {
                            return server.FindRoles(rank, false).First();
                            
                            
                        }
                    }
                }
                
            }
            if (settingsRepo.RoleByParameter(server.Id))
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
                foreach (string role in filter)
                {
                    if (role.ToLower().Contains(parameter.ToLower()))
                    {
                        try
                        {
                            ulong id = settingsRepo.GetOverride(role.ToLower(), server.Id);
                            r = server.GetRole(id);
                            if (r == null)
                            {
                                throw new Exception();
                            }

                        }
                        catch
                        {
                            return server.FindRoles(role, false).First();

                        }
                    }
                }


            }

            if (settingsRepo.RegionByParameter(server.Id))
            {
               
                foreach (string region in new RegionRepo(new RegionContext()).GetAllRegions())
                {
                    if (parameter.ToLower() == region.ToLower())
                    {
                        try
                        {
                            
                            return server.GetRole(settingsRepo.GetOverride(region.ToLower(), server.Id));
                            


                        }
                        catch
                        {
                            var temp = server.FindRoles(region, false).First();
                            if (settingsRepo.IsRoleDisabled(temp.Name.ToLower(), server.Id))
                            {
                                
                            }
                            else
                            {
                                return temp;
                                
                            }
                            
                        }
                    }
                }
            }
            foreach (string role in settingsRepo.GetAllOverrides(server.Id))
            {
                var temp = server.GetRole(Convert.ToUInt64(role.Substring(role.IndexOf(":") + 1, role.Length - role.IndexOf(":") - 1)));
                if (parameter.ToLower() == temp.Name.ToLower())
                {
                    return temp;

                }
            }
            return r;
        }

        public void UniversalRole()
        {
            commands.CreateCommand("Roles")
                .Parameter("Parameter", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring;
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    if (serverRepo.IsServerVerified(e.Server.Id))
                    {
                        try
                        {
                            Discord.Role r = RoleSearch(e.Server, e.GetArg("Parameter").ToLower());
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
                            returnstring = Eng_Default.RoleNotFound(e.GetArg("Parameter"));
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

