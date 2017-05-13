using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.Roles;
using AtlasBot.Modules.Role_Management;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Languages;
using RiotLibary.Roles;
using RiotSharp.SummonerEndpoint;
using ToolKit;

namespace AtlasBot.Modules.User
{
    public class AccountManagementCommands
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public AccountManagementCommands(DiscordClient botUser, CommandService commands)
        {
            this.BotUser = botUser;
            this.commands = commands;
        }
        public void ClaimAccount()
        {
            commands.CreateCommand("ClaimAccount")
                .Parameter("Region", ParameterType.Required)
                .Parameter("Summoner", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnmessage = "An error happened.";
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        RiotSharp.Region region = LeagueAndDatabase.GetRegionFromString(e.GetArg("Region"));
                        string summonername = e.GetArg("Summoner");
                        SummonerRepo sumRepo = new SummonerRepo(new SummonerContext());
                        UserRepo userRepo = new UserRepo(new UserContext());
                        int riotid = Convert.ToInt32(new SummonerAPI().GetSummonerId(summonername, region));
                        string token = new StringBuilder().CreateToken();
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
                                    new RoleManagementCommands(BotUser, commands).GetRoles(e.Server, e.User);
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

        public void Claim()
        {
            commands.CreateCommand("ClaimAccount")
                .Do(async (e) =>
                {
                   Summoner summoner =
                        new SummonerAPI().GetSummoner(
                            new SummonerRepo(new SummonerContext()).GetUnverifiedSummonerByUserId(e.User.Id),
                            ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                                new RegionRepo(new RegionContext()).GetRegionId(new UserRepo(new UserContext()).GetUserByDiscord(e.User.Id))
                            ));
                    SummonerRepo sumRepo = new SummonerRepo(new SummonerContext());
                    UserRepo userRepo = new UserRepo(new UserContext());
                    string returnmessage = "An error happened.";
                    if (new ServerRepo(new ServerContext()).IsServerVerified(e.Server.Id))
                    {
                        returnmessage =
                                Eng_Default.RenameMasteryPageLong(
                                    sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), Convert.ToInt32(summoner.Id)));
                        string token2 = sumRepo.GetToken(userRepo.GetUserByDiscord((e.User.Id)), Convert.ToInt32(summoner.Id));
                        foreach (var page in new SummonerAPI().GetSummonerMasteryPages(summoner.Name, summoner.Region))
                        {
                            if (page.Name.ToLower() == token2.ToLower())
                            {
                                sumRepo.VerifySummoner(userRepo.GetUserByDiscord((e.User.Id)), Convert.ToInt32(summoner.Id));
                                returnmessage = Eng_Default.AccountVerified();
                                new RoleManagementCommands(BotUser, commands).GetRoles(e.Server, e.User);
                            }
                        }
                    }
                    await e.Channel.SendMessage(returnmessage);
                });
        }
    }
}

