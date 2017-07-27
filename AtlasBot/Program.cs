using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.Modules.Administrative;
using AtlasBot.Modules.Coach;
using AtlasBot.Modules.Fun;
using AtlasBot.Modules.Help;
using AtlasBot.Modules.Logging;
using AtlasBot.Modules.Mastery;
using AtlasBot.Modules.Matchmaking;
using AtlasBot.Modules.Rank;
using AtlasBot.Modules.Region;
using AtlasBot.Modules.Roles;
using AtlasBot.Modules.Role_Management;
using AtlasBot.Modules.Server;
using AtlasBot.Modules.Server_Info;
using AtlasBot.Modules.User;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;

namespace AtlasBot
{
    public class Program
    {

        public static void Main(string[] args)
        {
            new Bot();
        }

        public class Bot
        {
            public DiscordClient BotUser;
            public CommandService commands;

            public Bot()
            {

                BotUser = new DiscordClient(x =>
                {
                    x.LogLevel = LogSeverity.Info;

                });
                BotUser.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

                BotUser.UsingCommands(x =>
                {
                    x.PrefixChar = '-';
                    x.AllowMentionPrefix = true;

                });
                commands = BotUser.GetService<CommandService>();
                RoleManagementCommands rmc = new RoleManagementCommands(BotUser, commands);
                AccountManagementCommands amc = new AccountManagementCommands(BotUser, commands);
                RoleManagementTrigger roleManagementTrigger = new RoleManagementTrigger(BotUser, commands);
                RegionCommands regionCommands = new RegionCommands(BotUser, commands);
                RoleCommand roleCommand = new RoleCommand(BotUser, commands);
                ServerInfoCommands serverInfoCommands = new ServerInfoCommands(BotUser, commands);
                MasteryCommands masteryCommands = new MasteryCommands(BotUser, commands);
                ServerManagement serverManagement = new ServerManagement(BotUser, commands);
                RankCommands rankCommands = new RankCommands(BotUser, commands);
                SummonerInfo summonerInfo = new SummonerInfo(commands);
                CreateRoles createRoles = new CreateRoles(commands);
                Matchmaking_Settings matchmakingSettings = new Matchmaking_Settings(commands);
                CoachCommands coachCommands = new CoachCommands(commands);
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                MatchmakingTrigger trigger = new MatchmakingTrigger(BotUser, commands);
                MatchmakingCommands matchmakingCommands = new MatchmakingCommands(commands, BotUser, trigger);
                new HelpCommand(commands);
                new ManagementTools(commands);
                new Universal_Role(BotUser, commands).UniversalRole();
                new BotManagement(commands, BotUser);
                new Interaction(BotUser, commands);
                new AltAccountCommand(commands).CreateCommands();
                //Task.Run(() => trigger.TimedClear(stopwatch));
                matchmakingCommands.CreateCommands();
                matchmakingSettings.ChannelSettings();
                coachCommands.CreateCommands();
                createRoles.CreateRank();
                //summonerInfo.SelfInfo();
                //summonerInfo.OtherInfo();
                serverManagement.ServerAdded();
                serverManagement.VerifyServer();
                serverInfoCommands.InviteLink();
                amc.ClaimAccount();
                amc.Claim();
                rankCommands.GetRank();
                regionCommands.GetRegion();
                roleCommand.GetRole();
                rmc.Update();
                rmc.GetRoles();
                serverInfoCommands.ServerInfo();
                serverInfoCommands.Description();
                serverManagement.CheckForNewServer();
                roleCommand.GetRoleParameter();
                Legal();
                roleManagementTrigger.JoiningRoleGive();
                masteryCommands.GetMasteryPoints();

                Test();
                BotUser.ExecuteAndWait(async () =>
                {
                    await BotUser.Connect(global::Keys.Keys.discordKey, TokenType.Bot);
                });
            }
            public void Legal()
            {
                commands.CreateCommand("legal")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendMessage("AtlasBot isn’t endorsed by Riot Games and doesn’t reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc. League of Legends © Riot Games, Inc");
                    });
            }

            public void Test()
            {
                commands.CreateCommand("Test")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendMessage(e.Server.GetChannel(new SettingsContext().GetLfgChannel(e.Server.Id)).Mention);
                    });
            }
        }
    }
}