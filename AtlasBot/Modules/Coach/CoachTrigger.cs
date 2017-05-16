using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace AtlasBot.Modules.Coach
{
    public class CoachTrigger
    {
        private CommandService commands;

        public CoachTrigger(CommandService commands)
        {
            this.commands = commands;
        }

    }
}
