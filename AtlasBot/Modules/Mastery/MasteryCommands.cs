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

namespace AtlasBot.Modules.Mastery
{
    public class MasteryCommands
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public MasteryCommands(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }
        public void GetMasteryPoints()
        {
            commands.CreateCommand("Mastery")
                .Do(async (e) =>
                {
                    string returnstring = "";
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
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

                    }
                    if (settingsRepo.MasteryPointsByAccount(e.Server.Id))
                    {
                        if (summoner != null)
                        {
                            int points = new MasteryAPI().GetPoints(summoner,
                                new ChampionAPI().GetChampion(settingsRepo.GetChampionId(e.Server.Id), RiotSharp.Region.eune));
                            Discord.Role r = e.Server.GetRole(settingsRepo.GetRoleByPoints(e.Server.Id, points));
                            await e.User.AddRoles(r);
                            returnstring = Eng_Default.RoleHasBeenGiven(r.Name);
                        }
                        else
                        {
                            returnstring = Eng_Default.RegisterAccount();
                        }
                    }
                    else
                    {
                        returnstring = Eng_Default.ServerDoesNotAllow();
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
