using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AtlasBot;
using Discord;
using Discord.Commands;

namespace WebUI
{
    public static class BotController
    {
        public static WebBot atlasBot;
        private static bool botOn = false;

        public static DiscordClient AtlasBot
        {
            get
            {
                while (atlasBot == null)
                {
                    if (!botOn)
                    {
                        ThreadStart child = new ThreadStart(StartBot);
                        Thread botThread = new Thread(child);
                        botThread.Start();
                        botOn = true;

                    }
                }
                return atlasBot.BotUser;
            }
        }

        public static void StartBot()
        {

            atlasBot = new WebBot();
        }
    }

    public class WebBot
    {
        public DiscordClient BotUser;
        public CommandService commands;

        public WebBot()
        {

            BotUser = new DiscordClient();
            Connect();
        }

        public async void Connect()
        {
            //await BotUser.Connect(Keys.Keys.discordKey, TokenType.Bot);
        }
    }
}


