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

        public async void QueuePerson(Summoner summoner, Discord.User user, Discord.Server currentserver)
        {
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            string queuemessage = "***" + user + " from " + currentserver.Name + " queued up as: ***\n";
            queuemessage += new User.SummonerInfo(commands).GetInfoShort(summoner);
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
            }
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
            SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
            
                
                    Discord.Channel channel = server.GetChannel(settingsRepo.GetLfgChannel(server.Id));
                    Discord.Message[] temp = await channel.DownloadMessages(100);
                    try
                    {
                        while (temp.Length > 0)
                        {
                            Discord.Message[] messages;
                            messages = await channel.DownloadMessages(100);
                            await channel.DeleteMessages(messages);
                            temp = await channel.DownloadMessages(100);
                        }
                        await channel.SendMessage("Queue has been cleared!");
                    }
                    catch
                    {
                await channel.SendMessage("Queue has been cleared!");
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
                if (stopwatch.Elapsed.TotalMinutes == minutes)
                {
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    foreach (var server in BotUser.Servers)
                    {
                        if (settingsRepo.lfgStatus(server.Id))
                        {
                            RemoveMessages(server);
                        }
                    }
                    stopwatch.Restart();
                }
            }
        }
        }

    }

