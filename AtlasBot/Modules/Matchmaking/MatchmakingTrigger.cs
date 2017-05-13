using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;

namespace AtlasBot.Modules.Matchmaking
{
    public class MatchmakingTrigger
    {
        private DiscordClient BotUser;

        public MatchmakingTrigger(DiscordClient BotUser)
        {
            this.BotUser = BotUser;

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
                    }
                    catch
                    {
                        
                    }
                    
          }

        public async void TimedClear(Stopwatch stopwatch)
        {
            int minutes = 1;
            while (stopwatch.IsRunning)
            {
                if (stopwatch.Elapsed.TotalMinutes == minutes)
                {
                    await BotUser.GetServer(305291793635999744).DefaultChannel.SendMessage("It's been 5 minutes!");
                    stopwatch.Restart();
                }
            }
        }
        }

    }

