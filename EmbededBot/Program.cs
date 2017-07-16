﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Novacode;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;
using Header = System.Runtime.Remoting.Messaging.Header;
using Microsoft.Office.Interop.Word;
using Task = System.Threading.Tasks.Task;

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
                await message.Channel.TriggerTypingAsync();
                CoachRepo coachRepo = new CoachRepo(new CoachContext());
                ChampionAPI championApi = new ChampionAPI();
                if (message.Content.ToLower().Contains("-coach list"))
                {
                    var coachlist = CoachlistHolder.List;
                    string returnmessage = GetList(coachlist);
                    await message.Channel.SendMessageAsync(returnmessage);

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
                        await message.Channel.SendMessageAsync(returnmessage, false, embed);
                    }
                    else
                    {
                        int championid = 0;
                        try
                        {
                            championid =
                                championApi.GetChampionId(message.Content.Remove(0, message.Content.IndexOf(' ') + 1));
                            await message.Channel.SendMessageAsync(GetList(coachRepo.GetCoachByChampion(championid)));
                        }
                        catch
                        {
                            await message.Channel.SendMessageAsync(GetList(coachRepo.GetCoachByRole(message.Content.Remove(0, message.Content.IndexOf(' ') + 1))));
                        }
                            
                        


                    }
                    
                }
            }
            else if (message.Content.ToLower() == "-export")
            {
                string filepath = ExportFile();
                await message.Channel.SendFileAsync(filepath);
            }
            else if (message.Content.ToLower() == "-deport bort")
            {
                await message.Channel.SendMessageAsync(message.Author.Mention +
                                                       ", sorry can not get rid of the creator. Must obay to not start Skynet.");
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
            if (!string.IsNullOrEmpty(champions))
            {
                coachInformation += string.Format("{0,-29}{1}", "**Champions:**", champions);
                coachInformation += "\n";
            }

            string language = "";
            coach.Languages.ForEach(l => language += l + ", ");
            try
            {
                language = language.Remove(language.Length - 2, 2);
            }
            catch
            {
            }

            if (!string.IsNullOrEmpty(language))
            {
                coachInformation += string.Format("{0,-27}{1}", "**Language(s):  **", " " + language);
                coachInformation += "\n";
            }

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
                    .AddField(new EmbedFieldBuilder().WithName("Coaching information:")
                        .WithValue(coachInformation))

                    .WithColor(new Color(0, 100, 0))
                ;
            if (user != null)
            {
                embed.WithAuthor(new EmbedAuthorBuilder().WithName(user.ToString()))
                    .WithThumbnailUrl(user.GetAvatarUrl());
            }
            else
            {
                embed.WithAuthor(
                    new EmbedAuthorBuilder().WithName(new UserRepo(new UserContext()).GetBackupName(coach.DiscordId)));
            }
            if (!string.IsNullOrEmpty(summmonerInfo))
            {
                embed.AddField(new EmbedFieldBuilder().WithName("Summoner information:")
                    .WithValue(summmonerInfo));
            }
            if (!string.IsNullOrEmpty(coach.Bio))
            {
                embed.AddField(new EmbedFieldBuilder().WithName("Bio").WithValue(coach.Bio));
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

        public string GetList(List<Coach> coachlist)
        {
            ChampionAPI championApi = new ChampionAPI();
            string coaches = "\n```http\nThese are all of our coaches: \n";
            coaches += string.Format("{0,-4}{1,-35}{2,-30}{3,-30}{4,-20}{5}", "Id", "Name", "Champion(s)",
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
                string name = "Error";
                try
                {
                    name = client.GetUser(coach.DiscordId).ToString();
                }
                catch
                {
                    name = new UserRepo(new UserContext()).GetBackupName(coach.DiscordId);/*Get Name From Database*/
                }

                coaches += string.Format("{0,-4}{1,-35}{2,-30}{3,-30}{4,-20}{5}", coach.Id + ":", name,
                    champions, roles, languages, verified);
                coaches += "\n";
            }
            coaches += "\n```";
            return coaches;
        }

        public string ExportFile()
        {
            //string filepath = @"C:\Users\bartd\Desktop\Exports\Export.txt";
            //List<string> content = new List<string>();
            //content.Add("Coaching file export by AtlasBot");
            //var coachlist = new CoachRepo(new CoachContext()).GetAllCoaches();
            //content.Add("Coach list for League of Mentoring as of " + DateTime.Now.ToLongDateString());
            //content.Add("---------------------------------------------------------------------");
            //foreach (var coach in coachlist)
            //{
            //    content.Add(new UserRepo(new UserContext()).GetBackupName(coach.DiscordId) + ":");
            //    content.Add(string.Format("{0,-30}{1}", "Id", coach.Id));
            //    content.Add(string.Format("{0,-30}{1}", "Discordid", coach.DiscordId));
            //    content.Add("---------------------------------------------------------------------");
            //}
            //StreamWriter sw = new StreamWriter(filepath);
            //foreach (var line in content)
            //{
            //    sw.WriteLine(line);
            //}
            //sw.Close();
            string filepath = @"C:\Users\bartd\Desktop\Exports\Export.docx";
            var doc = DocX.Create(filepath);
            string headerText = "Coach list for League of Mentoring as of " + DateTime.Now.ToLongDateString();
            var titleFormat = new Formatting();
            titleFormat.FontFamily = new System.Drawing.FontFamily("Arial Black");
            titleFormat.Size = 18D;
            titleFormat.Position = 12;
            var paraFormat = new Formatting();
            paraFormat.FontFamily = new System.Drawing.FontFamily("Calibri");
            paraFormat.Size = 10D;
            titleFormat.Position = 12;
            var coachFormat = new Formatting();
            coachFormat.FontFamily = new System.Drawing.FontFamily("Arial Black");
            coachFormat.Size = 15D;
            coachFormat.Position = 12;
            Novacode.Paragraph title = doc.InsertParagraph(headerText, false, titleFormat);
            title.Alignment = Alignment.center;
            var coachlist = new CoachRepo(new CoachContext()).GetAllCoaches();
            Novacode.Table table = doc.AddTable(coachlist.Count +1, 3);
            table.Rows[0].Cells[0].Paragraphs.First().AppendLine("Id");
            table.Rows[0].Cells[1].Paragraphs.First().AppendLine("Name");
            table.Rows[0].Cells[2].Paragraphs.First().AppendLine("DiscordId");
            int x = 1;
            UserRepo userRepo = new UserRepo(new UserContext());
            foreach (var coach in coachlist)
            {
                table.Rows[x].Cells[0].Paragraphs.First().AppendLine(coach.Id.ToString());
                table.Rows[x].Cells[1].Paragraphs.First().AppendLine(userRepo.GetBackupName(coach.DiscordId));
                table.Rows[x].Cells[2].Paragraphs.First().AppendLine(coach.DiscordId.ToString());
                x++;
            }
            ////doc.InsertTableOfContents("Test", TableOfContentsSwitches.None); ITS POSSSIBLE YAY
            doc.InsertTable(table);
            //foreach (Coach coach in new CoachRepo(new CoachContext()).GetAllCoaches())
            //{
            //    doc.InsertParagraph(new UserRepo(new UserContext()).GetBackupName(coach.DiscordId) + ":", false, coachFormat);
            //    //string content = "";
            //    Novacode.Table table = doc.AddTable(2, 2);
            //    table.Rows[0].Cells[0].Paragraphs.First().AppendLine("Id");
            //    table.Rows[0].Cells[1].Paragraphs.First().AppendLine(coach.Id.ToString());
            //    table.Rows[1].Cells[0].Paragraphs.First().AppendLine("Discord Id");
            //    table.Rows[1].Cells[1].Paragraphs.First().AppendLine(coach.DiscordId.ToString());
            //    doc.InsertTable(table);
            //    //content += string.Format("{0,-30}{1}", "Id", coach.Id);
            //    //content += "\n";
            //    //content += string.Format("{0,-30}{1}", "Discordid", coach.DiscordId);
            //    //Novacode.Paragraph coachtext = doc.InsertParagraph(content, false, paraFormat);
            //}
            doc.Save();
            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = appWord.Documents.Open(filepath);
            wordDocument.ExportAsFixedFormat(Path.ChangeExtension(filepath, ".pdf"), WdExportFormat.wdExportFormatPDF);
            wordDocument.Close();
            appWord.Quit();
            return Path.ChangeExtension(filepath, ".pdf");
        }
    }

    public static class CoachlistHolder
    {
        private static List<Coach> list;
        private static bool upToDate = false;

        public static List<Coach> List
        {
            get
            {
                if (list == null)
                {
                    Update();
                    upToDate = true;
                }
                if (upToDate == false)
                {
                    Update();
                    upToDate = true;
                }
                return list;
            }
            set { list = value; }
        }

        public static void Update()
        {
            List = new CoachRepo(new CoachContext()).GetAllCoaches();
        }

    }
}
