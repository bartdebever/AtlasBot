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
        }
        public string GetInfoShort(Summoner summoner)
        {
            StaticRiotApi staticApi = StaticRiotApi.GetInstance(Keys.Keys.riotKey);
            string returnstring = "";
            returnstring += "**"+summoner.Name + ": **";
            returnstring += "\n**Region:** " + summoner.Region.ToString().ToUpper();
            returnstring += "\n**Level:** " + summoner.Level.ToString();
            if (summoner.Level == 30)
            {
                RankAPI rankApi = new RankAPI();
                try
                {
                    returnstring += "\n**Rankings: **";
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedSolo5x5) != null) returnstring += "\nSolo: " + rankApi.GetRankingHarder(summoner, Queue.RankedSolo5x5);
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedFlexSR) != null) returnstring += "\nFlex: " + rankApi.GetRankingHarder(summoner, Queue.RankedFlexSR);
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedFlexTT) != null) returnstring += "\n3v3: " + rankApi.GetRankingHarder(summoner, Queue.RankedFlexTT); 
                }
                catch { }
            }
            try
            {
                int gamesplayed = new RoleAPI().GetGamesPlayed(summoner);
                if (gamesplayed != 0) returnstring += "**\nTotal Solo Games Played:** " + gamesplayed + " games";
                var champList = new ChampionAPI().Get5MainChampions(summoner);
                returnstring += "**\nMain ranked champions:**";
                foreach (var champ in champList)
                {
                    returnstring += "\n"+champ.Name + ": " + champ.Count + " games.";
                }
            }
            catch { }
            try
            {
                var masteryList = new MasteryAPI().GetChampionMasterys(summoner);
                masteryList  = masteryList.OrderBy(c => c.ChampionLevel).ThenBy(c => c.ChampionPoints).ToList();
                masteryList.Reverse();
                int champions = 3;
                if (masteryList.Count < 3)
                {
                    champions = masteryList.Count;
                }
                returnstring += "\n**Highest mastery:** ";
                for (int i = 0; i < champions; i++)
                {
                    returnstring += "\n" + staticApi.GetChampion(RiotSharp.Region.br, (Convert.ToInt32((masteryList[i].ChampionId)))).Name + ": Level " + masteryList[i].ChampionLevel.ToString() + ", " + masteryList[i].ChampionPoints.ToString() + " Points";
                }
            }
            catch { }
            
            return returnstring;
        }

        public void SelfInfo()
        {
            commands.CreateCommand("Info")
                .Do(async (e) =>
                {
                    await e.Channel.SendIsTyping();
                    string returnstring = Eng_Default.RegisterAccount();
                    try
                    {
                        DataLibary.Models.User user =
                                    new UserRepo(new UserContext()).GetUserByDiscord(e.User.Id);
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
    }
}
