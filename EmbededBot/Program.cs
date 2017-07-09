using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;

namespace EmbededBot
{
    class Program
    {
        private DiscordSocketClient client;
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
            Console.ReadLine();
        }
        public async Task MainAsync()
        {
            client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            string token = Keys.Keys.discordKey; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.FromResult(false);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.Contains("-info"))
            {
                try
                {
                    User user = new UserRepo(new UserContext()).GetUserByDiscord(message.Author.Id);
                    Summoner summoner = new SummonerAPI().GetSummoner(user.RiotId, user.Region);
                    Embed embed = new EmbedBuilder().WithColor(Color.Default)
                        .WithThumbnailUrl("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/profileicon/" +
                                          summoner.ProfileIconId + ".png")
                        .WithAuthor(new EmbedAuthorBuilder()
                            .WithName(message.Author.Username.ToString() + "'s account:")
                            .WithUrl("http://" + summoner.Region + ".op.gg/summoner/userName=" + summoner.Name))
                        //.WithDescription("\n**Name:** Bort**Rank:** " + new RankAPI().GetRankingHarder(summoner, Queue.RankedSolo5x5));
                        .WithDescription(GetInfoShort(summoner));
                    await message.Channel.SendMessageAsync("", false, embed);

                }
                catch(Exception ex)
                {
                    await message.Channel.SendMessageAsync(ex.Message);
                }


            }
            else if (message.Content.ToLower().Contains("-coach"))
            {
                CoachRepo coachRepo = new CoachRepo(new CoachContext());
                ChampionAPI championApi = new ChampionAPI();
                if (message.Content.ToLower().Contains("-coach list"))
                {
                    var coachlist = coachRepo.GetAllCoaches();
                    string coaches = "\n```http\nThese are all of our coaches: \n";
                    coaches += string.Format("{0,-4}{1,-30}{2,-30}{3,-30}{4,-30}{5}", "Id", "Name", "Champion(s)",
                        "Role(s)",
                        "Language(s)", "Verified");
                    coaches += "\n";
                    foreach (var coach in coachlist)
                    {
                        string roles = "";
                        //coach.Roles.ForEach(r => roles += r + ", ");

                        string champions = "";
                        string languages = "";
                        int x = 0;
                        bool loop = true; //need better name
                        while (loop || x <= coach.ChampionIds.Count)
                        {
                            var champname = "";
                            try
                            {
                                champname = championApi.GetChampionName(coach.ChampionIds[x]) + ", ";
                            }
                            catch
                            {
                                loop = false;
                            }

                            if (champions.Length + champname.Length < 30)
                            {
                                champions += champname;
                            }
                            else
                            {
                                loop = false;
                            }
                            x++;
                        }
                        x = 0;
                        loop = true;
                        while (loop || x <= coach.Roles.Count)
                        {
                            var rolename = "";
                            try
                            {
                                rolename = coach.Roles[x] + ", ";
                            }
                            catch
                            {
                                loop = false;
                            }

                            if (roles.Length + rolename.Length < 30)
                            {
                                roles += rolename;
                            }
                            else
                            {
                                loop = false;
                            }
                            x++;
                        }
                        x = 0;
                        loop = true;
                        while (loop || x <= coach.Languages.Count)
                        {
                            var rolename = "";
                            try
                            {
                                rolename = coach.Languages[x] + ", ";
                            }
                            catch
                            {
                                loop = false;
                            }

                            if (languages.Length + rolename.Length < 30)
                            {
                                languages += rolename;
                            }
                            else
                            {
                                loop = false;
                            }
                            x++;
                        }
                        try
                        {
                            roles = roles.Remove(roles.Length - 2, 2);
                        }
                        catch
                        {
                        }
                        try
                        {
                            languages = languages.Remove(languages.Length - 2, 2);
                        }
                        catch
                        {
                        }
                        try
                        {
                            champions = champions.Remove(champions.Length - 2, 2);
                        }
                        catch
                        {
                        }
                        string verified = "-";
                        if (coach.LoMVerified)
                        {
                            verified = "League of Mentoring";
                        }
                        string name = "BACKUP";
                        try
                        {
                            name = client.GetUser(coach.DiscordId).ToString();
                        }
                        catch
                        {
                            /*Get Name From Database*/
                        }

                        coaches += string.Format("{0,-4}{1,-30}{2,-30}{3,-30}{4,-30}{5}", coach.Id + ":", name,
                            champions, roles, languages, verified);
                        coaches += "\n";
                    }
                    coaches += "\n```";
                    await message.Channel.SendMessageAsync(coaches);
                }
                else
                {
                    Embed embed = null;
                    string returnmessage = "";
                    int id = 0;
                    try { id = Convert.ToInt32(message.Content.Split(' ')[1]); } catch { }
                    if (id != 0)
                    {
                        embed = GetCoachInformation(coachRepo.GetAllCoaches().Single(c => c.Id == id));
                    }
                    else
                    {
                        returnmessage = "Coach not found with id " + message.Content.Split(' ')[1];
                    }
                    await message.Channel.SendMessageAsync(returnmessage, false, embed);
                }
            }
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
                        returnstring += String.Format("{0,-3}{1, -27}{2}", "\n", "Solo: ", rankApi.GetRankingHarder(summoner, Queue.RankedSolo5x5));
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedFlexSR) != null)
                        returnstring += String.Format("{0,-3}{1, -27}{2}", "\n", "Flex: ", rankApi.GetRankingHarder(summoner, Queue.RankedFlexSR));
                    if (rankApi.GetRankingHarder(summoner, Queue.RankedFlexTT) != null)
                        returnstring += String.Format("{0,-3}{1, -27}{2}", "\n", "3v3: ", rankApi.GetRankingHarder(summoner, Queue.RankedFlexTT));
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
                        String.Format("{0,-3}{1, -27}{2,-10}{3}", "\n", staticApi.GetChampion(RiotSharp.Region.br,
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

        public Embed GetCoachInformation(Coach coach)
        {

            SocketUser user = null;
            try{user = client.GetUser(coach.DiscordId); } catch { }
            string summmonerInfo = "";
            try
            {
                Summoner summoner = new SummonerAPI().GetSummoner(coach.SummonerId, coach.region);
                summmonerInfo += string.Format("{0,-24}{1}", "**Summoner Name:**", summoner.Name);
                summmonerInfo += "\n";
                summmonerInfo += string.Format("{0,-36}{1}", "**Region:**", " " + summoner.Region.ToString().ToUpper());
                summmonerInfo += "\n";
                try
                {
                    summmonerInfo += string.Format("{0,-37}{1}", "**Rank:**", " " +
                                                                              new RankAPI().GetRankingHarder(summoner, Queue.RankedSolo5x5));
                }
                catch
                {
                    summmonerInfo += string.Format("{0,-37}{1}", "**Level:**", summoner.Level);
                }
            }
            catch { }



            string coachInformation = "";
            string roles = "";
            coach.Roles.ForEach(r => roles += r + ", ");
            try
            {
                roles = roles.Remove(roles.Length - 2, 2);
            }
            catch
            {
            }
            coachInformation += string.Format("{0,-36}{1}", "**Roles:**", roles);
            coachInformation += "\n";
            string champions = "";
            coach.ChampionIds.ForEach(c => champions += new ChampionAPI().GetChampionName(c) + ", ");
            try
            {
                champions = champions.Remove(champions.Length - 2, 2);
            }
            catch
            {
            }
            coachInformation += string.Format("{0,-29}{1}", "**Champions:**", champions);
            string language = "";
            coach.Languages.ForEach(l => language += l + ", ");
            try
            {
                language = language.Remove(language.Length - 2, 2);
            }
            catch
            {
            }
            coachInformation += "\n";
            coachInformation += string.Format("{0,-27}{1}", "**Language(s):  **", " " + language);
            coachInformation += "\n";
            string preference = "";
            coach.Prerferences.ForEach(p => preference += p + ", ");
            try
            {
                preference = preference.Remove(preference.Length - 2, 2);
            }
            catch
            {
            }
            if (!string.IsNullOrEmpty(preference))
            {
                coachInformation += string.Format("{0,-28}{1}", "**Preference(s):**", preference);
                coachInformation += "\n";
            }
            
            if (!string.IsNullOrEmpty(coach.Timezone))
            {
                coachInformation += string.Format("{0,-30}{1}", "**Timezone:  **", " " + coach.Timezone);
                coachInformation += "\n";
            }
            if (!string.IsNullOrEmpty(coach.Availability))
            {
                coachInformation += string.Format("{0,-31}{1}", "**Availability:**", " " + coach.Availability);
            }
            

            if (!String.IsNullOrEmpty(coach.Cost))
            {
                coachInformation += "\n";
                coachInformation += string.Format("{0,-37}{1}", "**Cost:**", coach.Cost);
            }
            string link = "";
            coach.Links.ForEach(l => link += l + "\n");
            var embed = new EmbedBuilder()
                    .WithFooter(new EmbedFooterBuilder().WithText("AtlasBot Coaching Module | " + DateTime.Now.ToLongTimeString()))
                    .AddField(new EmbedFieldBuilder().WithName("Summoner information:")
                        .WithValue(summmonerInfo))
                    .AddField(new EmbedFieldBuilder().WithName("Coaching information:")
                        .WithValue(coachInformation))

                    .WithColor(new Color(0, 100, 0))
                ;
            if (user != null)
            {
                embed.WithAuthor(new EmbedAuthorBuilder().WithName(user.ToString()))
                    .WithThumbnailUrl(user.GetAvatarUrl());
            }
            if (!string.IsNullOrEmpty(link))
                embed.AddField(new EmbedFieldBuilder().WithName("Personal Links").WithValue(link));
            if (coach.LoMVerified)
            {
                embed.AddField(new EmbedFieldBuilder().WithName("Verified!")
                    .WithValue(
                        "This coach has been verified by League of Mentoring, visit them at http://discord.gg/leagueofmentoring"));
            }
            return embed;
        }
    }
}
