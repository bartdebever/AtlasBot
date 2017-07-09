using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.Administrative;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Keys;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Matchmaking
{
    public class MatchmakingTrigger
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public MatchmakingTrigger(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }

        public async void QueuePerson(Summoner summoner, Discord.User user, Discord.Server currentserver, string queue)
        {
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            //string queuemessage = "***" + user + " from " + currentserver.Name + " queued up for "+ queue + " as: ***\n";
            //queuemessage += new User.SummonerInfo(commands).GetInfoShort(summoner);
            string queuemessage = GenerateMessage(summoner, user, currentserver, queue);
            foreach (Discord.Server server in BotUser.Servers)
            {
                
                if (settingsRepo.lfgStatus(server.Id))
                {
                    var channel = server.GetChannel(settingsRepo.GetLfgChannel(server.Id));
                    bool found = false;
                    foreach (var message in channel.DownloadMessages(100).Result)
                    {
                        if (message.Text.Contains(user.ToString()))
                        {
                            found = true;
                        }
                    }
                    if (found == false) await channel.SendMessage(queuemessage);
                }
                else if (server.Id == DiscordIds.AtlasId)
                {
                    foreach (var channel in server.TextChannels)
                    {
                        if (channel.Name.ToLower().Contains(summoner.Region.ToString().ToLower()) && channel.Name.ToLower().Contains("queue"))
                        {
                            await channel.SendMessage(queuemessage);
                        }
                    }
                }
            }
        }

        public string GenerateMessage(Summoner summoner, Discord.User user, Discord.Server currentserver, string queue)
        {
            string role = "-";
            queue = queue.First().ToString().ToUpper() + String.Join("", queue.Skip(1));
            string result = "```http\n";
            result += user.ToString() + " from " + currentserver.Name + ": ";
            string title = "Rank";
            result += "\n";
            if (summoner.Level != 30) title = "Level";
            result += String.Format("{0,-30}{1,-10}{2,-20}{3,-20}{4}", "Summoner", "Region", "Queue", title, "Main Role");
            string rank = summoner.Level.ToString();
            if (summoner.Level == 30)
            {
                role = new RoleAPI().GetRole(summoner);
                rank = new RankAPI().GetRankingHarder(summoner, Queue.RankedSolo5x5);
            }
            result += "\n";
            result += String.Format("{0,-30}{1,-10}{2,-20}{3,-20}{4}",summoner.Name, summoner.Region.ToString().ToUpper(), queue, rank, role);
            result += "\n```";
            return result;
        }
        public async void LeaveQueue(Discord.User user)
        {
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            foreach (Discord.Server server in BotUser.Servers)
            {

                if (settingsRepo.lfgStatus(server.Id))
                {
                    var channel = server.GetChannel(settingsRepo.GetLfgChannel(server.Id));
                    bool found = false;
                    foreach (var message in channel.Messages)
                    {
                        if (message.Text.Contains(user.ToString()))
                        {
                            await message.Delete();
                        }
                    }
                    
                }
            }
        }
        public async void RemoveMessages(Discord.Server server)
        {
            if (server.Id != DiscordIds.AtlasId)
            {
                SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                Discord.Channel channel = server.GetChannel(settingsRepo.GetLfgChannel(server.Id));
                Discord.Message[] temp = await channel.DownloadMessages(100);
                bool found = false;
                try
                {

                    while (temp.Length > 1 && temp.Last().Text != "queue has been cleared!")
                    {
                        await channel.DeleteMessages(temp);
                        found = true;
                        temp = await channel.DownloadMessages(100);

                    }
                }
                catch
                {
                    found = true;
                }
                if (found == true)
                {
                    await channel.SendMessage("Queue has been cleared!");
                }
            }
            else if (server.Id == DiscordIds.AtlasId)
            {
                List<Channel> channels = new List<Channel>();
                foreach (var channel in server.TextChannels)
                {
                    if (channel.Name.Contains("queue"))
                    {
                        channels.Add(channel);
                    }
                    
                }
                foreach (var channel in channels)
                {
                    Discord.Message[] temp = await channel.DownloadMessages();
                    bool found = false;
                    try
                    {

                        while (temp.Length > 1 && temp.Last().Text != "queue has been cleared!")
                        {
                            await channel.DeleteMessages(temp);
                            found = true;
                            temp = await channel.DownloadMessages();

                        }
                    }
                    catch
                    {
                        found = true;
                    }
                    if (found)
                    {
                        await channel.SendMessage("Queue has been cleared!");
                    }
                }
            }
            

        }

        public  void TimedClear(Stopwatch stopwatch)
        {
            int time = 0;
            int minutes = 30;
            while (stopwatch.IsRunning)
            {
                if (Convert.ToInt32(stopwatch.Elapsed.TotalMinutes) != time)
                {
                    time = Convert.ToInt32(stopwatch.Elapsed.TotalMinutes);
                    BotUser.SetGame((30 - time).ToString() + " minutes till queue reset!");
                    
                    
                }
                if (stopwatch.Elapsed.TotalMinutes >= minutes)
                {
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    foreach (var server in BotUser.Servers)
                    {
                        if (settingsRepo.lfgStatus(server.Id))
                        {
                            RemoveMessages(server);
                        }
                    }
                    stopwatch.Stop();
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
        }
        }

    }

