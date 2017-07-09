using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord.API.Client;
using Discord.Commands;
using Languages;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;
using Message = Discord.Message;

namespace AtlasBot.Modules.User
{
    public class SummonerInfo
    {
        private CommandService commands;

        public SummonerInfo(CommandService commands)
        {
            this.commands = commands;
            Test();
        }

        public string GetInfoShort(Summoner summoner)
        {
            StaticRiotApi staticApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
            string returnstring = "```http\n";

            returnstring += summoner.Name + ":\n";
            returnstring += String.Format("{0,-30}{1}", "\nRegion:", summoner.Region.ToString().ToUpper());
            returnstring += String.Format("{0,-30}{1}", "\nLevel:", summoner.Level.ToString());
            try
            {
                int gamesplayed = new RoleAPI().GetGamesPlayed(summoner);
                if (gamesplayed != 0)
                    returnstring += String.Format("{0,-30}{1}", "\nTotal Solo Games Played:",
                        gamesplayed + " games");
                var champList = new ChampionAPI().Get5MainChampions(summoner);
                returnstring += "\nMain ranked champions:";
                //foreach (var champ in champList)
                //{
                //    returnstring += String.Format("{0,-3}{1, -27}{2}", "\n", champ.Name, champ.Count + " games.");
                //}
                returnstring += String.Format("{0,-3}{1, -27}{2}", "\n", champList[0].Name + ":", champList[0].Count + " games.");
                returnstring += String.Format("{0,-3}{1, -27}{2}", "\n", champList[1].Name + ":", champList[1].Count + " games.");
            }
            catch
            {
            }
            if (summoner.Level == 30)
            {
                RankAPI rankApi = new RankAPI();
                try
                {
                    returnstring += "\nRankings:";
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedSolo5x5) != null)
                        returnstring += String.Format("{0,-3}{1, -27}{2}", "\n","Solo: ",rankApi.GetRankingHarder(summoner, Queue.RankedSolo5x5));
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedFlexSR) != null)
                        returnstring += String.Format("{0,-3}{1, -27}{2}", "\n","Flex: ",rankApi.GetRankingHarder(summoner, Queue.RankedFlexSR));
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedFlexTT) != null)
                        returnstring += String.Format("{0,-3}{1, -27}{2}", "\n","3v3: ", rankApi.GetRankingHarder(summoner, Queue.RankedFlexTT));
                }
                catch
                {
                }
            }

            try
            {
                var masteryList = new MasteryAPI().GetChampionMasterys(summoner);
                masteryList = masteryList.OrderBy(c => c.ChampionLevel).ThenBy(c => c.ChampionPoints).ToList();
                masteryList.Reverse();
                int champions = 3;
                if (masteryList.Count < 3)
                {
                    champions = masteryList.Count;
                }
                returnstring += "\nHighest mastery: ";
                for (int i = 0; i < champions; i++)
                {
                    returnstring +=
                        String.Format("{0,-3}{1, -27}{2,-10}{3}","\n", staticApi.GetChampion(RiotSharp.Region.br,
                            (Convert.ToInt32((masteryList[i].ChampionId)))).Name + ":", "Level " +
                        masteryList[i].ChampionLevel.ToString(), masteryList[i].ChampionPoints +
                        " Points");
                }
            }
            catch
            {
            }
            returnstring += "\n```";
            return returnstring;
        }

        public void SelfInfo()
        {
            commands.CreateCommand("Info")
                .Parameter("Id", ParameterType.Optional)
                .Do(async (e) =>
                {
                    await e.Channel.SendIsTyping();
                    string returnstring = Eng_Default.RegisterAccount();
                    try
                    {
                        //DataLibary.Models.User user =
                        //    new UserRepo(new UserContext()).GetUserByDiscord(e.User.Id);
                        //Summoner summoner =
                        //    new SummonerAPI().GetSummoner(
                        //        new SummonerRepo(new SummonerContext()).GetSummonerByUserId(user),
                        //        ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                        //            new RegionRepo(new RegionContext()).GetRegionId(user)
                        //        ));
                        //returnstring = GetInfoShort(summoner);
                        var accounts = new UserRepo(new UserContext()).GetAllAccounts(e.User.Id);
                        if (accounts.Count != 0)
                        {
                            if (accounts.Count == 1)
                            {
                                returnstring =
                                    GetInfoShort(new SummonerAPI().GetSummoner(accounts[0].RiotId, accounts[0].Region));
                            }
                            else
                            {
                                int id = 0;
                                try
                                {
                                     id = Convert.ToInt32(e.GetArg("Id"));
                                }
                                catch { }
                                if (id == 0)
                                {
                                    RankAPI rankApi = new RankAPI();
                                    returnstring = "```http\n";
                                    returnstring += e.User.ToString() + ": \n";
                                    returnstring += String.Format("{0,-30}{1}", "Joined At:", e.User.JoinedAt.ToLongDateString());
                                    returnstring += "\n";
                                    //returnstring += String.Format("{0,-30}{1}", "Avatar:", e.User.AvatarUrl);
                                    //returnstring += "\n";
                                    //if (e.User.CurrentGame.HasValue)
                                    //{
                                    //    returnstring += String.Format("{0,-30}{1}", "Currently playing: ",
                                    //        e.User.CurrentGame.Value.Name);
                                    //    returnstring += "\n";
                                    //}
                                    returnstring += "Accounts:\n\n";
                                    returnstring += e.User.Name + "\'s League of Legends Accounts: \n\n";
                                    returnstring += String.Format("{0,-4}{1,-7}{2, -25}{3,-10}{4,-15}{5}", "Id", "Region", "Summonername", "Level", "Rank", "Type");
                                    returnstring += "\n";
                                    int x = 1;
                                    foreach (var account in accounts)
                                    {
                                        Summoner summoner = new SummonerAPI().GetSummoner(account.RiotId, account.Region);
                                        string type = "Alternate";
                                        if (account.Main)
                                        {
                                            type = "Main";
                                        }
                                        returnstring += "\n" + String.Format("{0,-4}{1,-7}{2, -25}{3,-10}{4,-15}{5}", x, summoner.Region.ToString().ToUpper(), summoner.Name, summoner.Level, rankApi.GetRankingHarder(summoner, Queue.RankedSolo5x5), type);
                                        x++;
                                    }
                                    returnstring += "\n\n!To get more information about an account use -info <id>!";
                                    returnstring += "\n```";
                                }
                                else { returnstring = GetInfoShort(new SummonerAPI().GetSummoner(accounts[id-1].RiotId, accounts[id-1].Region)); }

                            }
                        }
                    }
                    catch
                    {

                    }
                    await e.Channel.SendMessage(returnstring);

                });
        }

        public void OtherInfo()
        {
            commands.CreateCommand("Info")
                .Parameter("User")
                .Do(async (e) =>
                {
                    string returnstring = Eng_Default.RegisterAccount();
                    try
                    {
                        DataLibary.Models.User user =
                            new UserRepo(new UserContext()).GetUserByDiscord(e.Message.MentionedUsers.First().Id);
                        Summoner summoner =
                            new SummonerAPI().GetSummoner(
                                new SummonerRepo(new SummonerContext()).GetSummonerByUserId(user),
                                ToolKit.LeagueAndDatabase.GetRegionFromDatabaseId(
                                    new RegionRepo(new RegionContext()).GetRegionId(user)
                                ));
                        returnstring = GetInfoShort(summoner);
                    }
                    catch
                    {

                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }

        public void Test()
        {
            commands.CreateCommand("TestCommand")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(new RankAPI().GetAllRankings());
                });
        }
    }
}

