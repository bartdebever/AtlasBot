using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.DataTypes;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord.Commands;
using Languages;
using RiotLibary.Roles;
using ToolKit;

namespace AtlasBot.Modules.User
{
    public class AltAccountCommand : Commands
    {
        public AltAccountCommand(CommandService commands) : base(commands)
        {
        }

        public override void CreateCommands()
        {
            AddAlt();   
        }

        private void AddAlt()
        {
            commands.CreateCommand("ClaimAlter")
                .Parameter("Region")
                .Parameter("SummonerName", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring;
                    SummonerAPI summonerApi = new SummonerAPI();
                    AltAccountRepo altAccountRepo = new AltAccountRepo(new AltAccountContext());
                    RiotSharp.Region region = LeagueAndDatabase.GetRegionFromString(e.GetArg("Region"));
                    long riotid = summonerApi.GetSummonerId(e.GetArg("SummonerName"), region);
                    if (altAccountRepo.UniverifiedAccount(e.User.Id)) //AltAccount in system
                    {
                        var token = altAccountRepo.GetToken(e.User.Id); // GetToken
                        bool found = false;
                        foreach (var page in summonerApi.GetRunePages(e.GetArg("SummonerName"), region))
                        {
                            if (page.Name.ToLower().Contains(token.ToLower())) found = true;
                        }
                        foreach (var page in summonerApi.GetSummonerMasteryPages(e.GetArg("SummonerName"), region))
                        {
                            if (page.Name.ToLower().Contains(token.ToLower())) found = true;
                        }
                        if (found)
                        {
                            altAccountRepo.VerifyAccount(e.User.Id);
                            returnstring = Eng_Default.AccountVerified();
                        }
                        else { returnstring = Eng_Default.RenameMasteryPageLong(token);}
                    }
                    else //Account is not in the system
                    {
                        var token = new StringBuilder().CreateToken();
                        altAccountRepo.AddAccount(e.User.Id, riotid, new RegionRepo(new RegionContext()).GetRegionId(region), token);
                        returnstring = Eng_Default.RenameMasteryPage(token);
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
