using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.DataTypes;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord.Commands;

namespace AtlasBot.Modules.Administrative
{
    public class CreateRoles : Commands
    {
        public CreateRoles(CommandService commands):base(commands)
        {
            this.commands = commands;
            CreateCommands();
        }
        public void CreateRank()
        {
            commands.CreateCommand("CreateRoles")
                .Do(async (e) =>
                {
                    SettingsRepo settingsRepo = new SettingsRepo(new SettingsContext());
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    if (serverRepo.IsServerVerified(e.Server.Id))
                    {
                        if (settingsRepo.RankByAccount(e.Server.Id) || settingsRepo.RankByParameter(e.Server.Id))
                        {
                            if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.Basic)
                            {
                                try
                                {
                                    await e.Server.CreateRole("Challenger", null, Color.Purple);
                                    await e.Server.CreateRole("Master", null, new Color(255, 165, 0));
                                    await e.Server.CreateRole("Diamond", null, new Color(0, 191, 255));
                                    await e.Server.CreateRole("Platinum", null, new Color(0, 255, 127));
                                    await e.Server.CreateRole("Gold", null, Color.Gold);
                                    await e.Server.CreateRole("Silver", null, Color.LightGrey);
                                    await e.Server.CreateRole("Bronze", null, new Color(139, 69, 19));
                                }
                                catch
                                {

                                }
                            }
                            else if (settingsRepo.RankCommandType(e.Server.Id) == CommandType.PerQueue)
                            {
                                await e.Server.CreateRole("Solo Challenger", null, Color.Purple);
                                await e.Server.CreateRole("Flex Challenger", null, Color.Purple);
                                await e.Server.CreateRole("3v3 Challenger", null, Color.Purple);
                                await e.Server.CreateRole("Solo Master", null, new Color(255, 165, 0));
                                await e.Server.CreateRole("Flex Master", null, new Color(255, 165, 0));
                                await e.Server.CreateRole("3v3 Master", null, new Color(255, 165, 0));
                                await e.Server.CreateRole("Solo Diamond", null, new Color(0, 191, 255));
                                await e.Server.CreateRole("Flex Diamond", null, new Color(0, 191, 255));
                                await e.Server.CreateRole("3v3 Diamond", null, new Color(0, 191, 255));
                                await e.Server.CreateRole("Solo Platinum", null, new Color(0, 255, 127));
                                await e.Server.CreateRole("Flex Platinum", null, new Color(0, 255, 127));
                                await e.Server.CreateRole("3v3 Platinum", null, new Color(0, 255, 127));
                                await e.Server.CreateRole("Solo Gold", null, Color.Gold);
                                await e.Server.CreateRole("Flex Gold", null, Color.Gold);
                                await e.Server.CreateRole("3v3 Gold", null, Color.Gold);
                                await e.Server.CreateRole("Solo Silver", null, Color.LightGrey);
                                await e.Server.CreateRole("Flex Silver", null, Color.LightGrey);
                                await e.Server.CreateRole("3v3 Silver", null, Color.LightGrey);
                                await e.Server.CreateRole("Solo Bronze", null, new Color(139, 69, 19));
                                await e.Server.CreateRole("Flex Bronze", null, new Color(139, 69, 19));
                                await e.Server.CreateRole("3v3 Bronze", null, new Color(139, 69, 19));
                            }
                        }
                        if (settingsRepo.RegionByAccount(e.Server.Id) || settingsRepo.RegionByParameter(e.Server.Id))
                        {
                            foreach (string line in new RegionRepo(new RegionContext()).GetAllRegions())
                            {
                                try
                                {
                                    Role r = e.Server.FindRoles(line).First();
                                    string test = r.Name;
                                }
                                catch
                                {
                                    try
                                    {
                                        await e.Server.CreateRole(line);
                                    }
                                    catch
                                    {
                                        //ignored
                                    }

                                }
                            }
                        }
                        await e.Channel.SendMessage("Roles created");
                    }
                });
            
        }

        public override void CreateCommands()
        {
            CreateRank();
        }
    }
}
