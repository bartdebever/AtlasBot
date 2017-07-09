using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord.Commands;
using Languages;
using RiotLibary.Roles;
using RiotSharp;
using RiotSharp.SummonerEndpoint;

namespace AtlasBot.Modules.Coach
{
    public class CoachCommands
    {
        private CommandService commands;

        public CoachCommands(CommandService commands)
        {
            this.commands = commands;
        }

        public void CreateCommands()
        {
            AddCoach();
            CoachTemplate();
            TestList();
        }

        private void AddCoach()
        {
            commands.CreateCommand("AddCoach")
                .Parameter("User")
                .Parameter("Role", ParameterType.Optional)
                .Parameter("ChampionName",ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string returnstring = "";
                    ServerRepo serverRepo = new ServerRepo(new ServerContext());
                    CoachTrigger trigger = new CoachTrigger();
                    if (e.Server.Id == 302775478824075267 && serverRepo.IsAdmin(e.User.Id, e.Server.Id))
                    {
                        
                            try
                            {
                                trigger.AddCoach(e.Message.MentionedUsers.First().Id, e.GetArg("ChampionName"),
                                    e.GetArg("Role"));
                                returnstring = "Coach added successfully.";
                            }
                            catch
                            {
                                returnstring = "Failed to add coach";
                            }
                        
                    }
                    else
                    {
                        returnstring = Eng_Default.NotAllowed();
                    }
                    await e.Channel.SendMessage(returnstring);
                });
        }

        private void CoachTemplate()
        {
            commands.CreateCommand("CoachTest")
                .Do(async (e) =>
                {
                    DataLibary.Models.Coach coach = new CoachRepo(new CoachContext()).GetAllCoaches().First();
                    Summoner summoner = new SummonerAPI().GetSummoner(coach.SummonerId, coach.region);
                    string returnstring = "```HTTP\n";
                    returnstring += "Mentoring information for " + summoner.Name + ": \n\n";
                    returnstring += string.Format("{0,-30}{1}", "Region:", summoner.Region.ToString().ToUpper());
                    string roles = "";
                    coach.Roles.ForEach(r => roles += r + ", ");
                    roles = roles.Remove(roles.Length - 2, 2);
                    returnstring += "\n";
                    returnstring += string.Format("{0,-30}{1}", "Role(s):", roles);
                    string champions = "";
                    coach.ChampionIds.ForEach(c => champions += new ChampionAPI().GetChampionName(c) +", ");
                    champions = champions.Remove(champions.Length - 2, 2);
                    returnstring += "\n";
                    returnstring += string.Format("{0,-30}{1}", "Main Champion(s):", champions);
                    returnstring += "\n";
                    returnstring += string.Format("{0,-30}{1}", "Ranks(s):", "S7: " + "Diamond III");
                    returnstring += "\n";
                    string language = "";
                    coach.Languages.ForEach(l => language += l + ", ");
                    language = language.Remove(language.Length - 2,2);
                    returnstring += string.Format("{0,-30}{1}", "Languages: ", language);
                    returnstring += "\n";
                    string preference = "";
                    coach.Prerferences.ForEach(p => preference += p +", ");
                    preference = preference.Remove(preference.Length - 2, 2);
                    returnstring += string.Format("{0,-30}{1}", "Coaching Preferences:",
                        preference);
                    returnstring += "\n";
                    returnstring += string.Format("{0,-30}{1}", "Availability/Timezone: ", coach.Availability + ", " + coach.Timezone);
                    returnstring += "\n\nPersonal Links:\n";
                    foreach (var link in coach.Links)
                    {
                        returnstring += string.Format("{0,-5}{1}", "", link+"\n");
                    }
                    if (coach.LoMVerified)
                    {
                        returnstring +=
                            "\n\nThis coach has been verified by League of Mentoring\nReach them at: DISCORDLINK";
                    }
                    returnstring += "\n```";
                    await e.Channel.SendMessage(returnstring);
                });
        }

        private void TestList()
        {
            commands.CreateCommand("CoachList")
                .Do(async (e) =>
                {
                    string returnstring = "```HTTP\n";
                    DataLibary.Models.Coach coach = new CoachRepo(new CoachContext()).GetAllCoaches().First();
                    returnstring += string.Format("{0,-4}{1,-20}{2,-10}{3,-40}{4,-30}{5}", "Id", "Discord","Region", "Languages", "Roles", "Champions");
                    returnstring += "\n";
                    returnstring += string.Format("{0,-4}{1,-20}{2,-10}{3,-40}{4,-30}{5}", coach.Id + ":", "Keonkwai#8458",coach.region.ToString().ToUpper(), "English, Other, Other", "AD Carry, Support", "Thresh, Lucian, Lee Sin");
                    returnstring += "\n";
                    returnstring += string.Format("{0,-4}{1,-20}{2,-10}{3,-40}{4,-30}{5}", 3 + ":", "Bort#9703","EUW", "English, Other, Other", "AD Carry, Support", "Thresh, Lucian, Lee Sin");
                    returnstring += "\n```";
                    await e.Channel.SendMessage(returnstring);
                });
        }
    }
}
