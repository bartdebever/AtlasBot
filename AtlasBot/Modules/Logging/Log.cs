using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasBot.Modules.Logging
{
    public class Log
    {
        DiscordClient BotUser;
        CommandService commands;
        public Log(DiscordClient client, CommandService commands)
        {
            this.BotUser = client;
            this.commands = commands;
        }
        public async void DMBort(string message)
        {
            await BotUser.GetServer(308946048800522240).FindUsers("Bort", true).First().SendMessage(message);
        }

        public void AdminLog(string message)
        {
            //BotUser.GetServer(291643233682063370).GetChannel(291643340678627328).SendMessage(message);
        }

    }
}
