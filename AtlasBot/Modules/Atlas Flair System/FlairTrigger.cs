using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.User;
using Discord;
using Discord.Commands;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Atlas_Flair_System
{
    public class FlairTrigger
    {
        private DiscordClient BotUser;
        private CommandService commands;

        public FlairTrigger(DiscordClient BotUser, CommandService commands)
        {
            this.BotUser = BotUser;
            this.commands = commands;
        }

        public async void CreateFlair(Summoner summoner)
        {
            Discord.Server server = BotUser.GetServer(227778876540059651 /* This is Atlas' server id*/);
            foreach (var channel in server.TextChannels)
            {
                if (channel.Name.ToLower().Contains(summoner.Region.ToString().ToLower())&& channel.Name.ToLower().Contains("flair"))
                {
                    await channel.SendMessage(new SummonerInfo(commands).GetInfoShort(summoner));
                }
            }
        }
    }
}
