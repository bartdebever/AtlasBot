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
using RiotLibary.Roles;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Role_Management
{
    public class RoleManagementTrigger
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public RoleManagementTrigger(DiscordClient client, CommandService commands)
        {
            this.BotUser = client;
            this.commands = commands;
        }
        public async void RemoveRoles(Discord.Server server, Discord.User discorduser)
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
                Console.WriteLine("User is not registered.");
            }

            if (summoner != null)
            {
                SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                List<string> filter = new List<string>();
                if (settingsRepo.RankCommandType(server.Id) == CommandType.Basic)
                {
                    filter = Ranks.BasicRanks();
                }
                else if (settingsRepo.RankCommandType(server.Id) == CommandType.PerQueue)
                {
                    filter = Ranks.QueueRanks();
                }
                else if (settingsRepo.RankCommandType(server.Id) == CommandType.Division)
                {
                    filter = Ranks.DivisionRanks();
                }
                List<Discord.Role> roles = new List<Discord.Role>();
                foreach (Discord.Role role in discorduser.Roles)
                {
                    foreach (string line in filter)
                    {
                        if (role.Name.ToLower() == line.ToLower())
                        {
                            roles.Add(role);
                        }
                        else
                        {
                            try
                            {
                                if (server.GetRole(settingsRepo.GetOverride(line.ToLower(), server.Id)).Id == role.Id)
                                {
                                    roles.Add(role);
                                }
                            }
                            catch
                            {
                                //no override
                            }
                        }

                    }
                }
                await server.GetUser(discorduser.Id).RemoveRoles(roles.ToArray());
                //foreach (string line in filter)
                //{
                //    Discord.Role r = null;
                //    try
                //    {
                //         r = server.GetRole(settingsRepo.GetOverride(line.ToLower(), server.Id));
                //    }
                //    catch
                //    {
                //        try
                //        {
                //            r = server.FindRoles(line, false).First();
                //        }
                //        catch { }

                //    }
                //    if (r != null)
                //    {
                //        if (discorduser.HasRole(r))
                //        {
                //            roles.Add(r);
                //        }   
                //    }
                //}
            }
        }
        public void JoiningRoleGive()
        {
            BotUser.UserJoined += async (s, u) =>
            {
                try
                {
                    new RoleManagementCommands(BotUser, commands).GetRoles(u.Server, u.User);
                    await u.User.SendMessage(u.Server.Name + " uses AtlasBot, your roles have been gifted automatically!");
                }
                catch
                {
                    Console.WriteLine("User not registered but joined");
                }
            };
        }
        public void OverrideDeletion(Discord.Server server)
        {
            string thisshouldntbeneededbutiguessitis = "";
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            foreach (string line in settingsRepo.GetAllOverrides(server.Id))
            {
                ulong id = Convert.ToUInt64(line.Split(':').Last());
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
    }
}
