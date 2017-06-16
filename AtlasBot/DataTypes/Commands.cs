using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace AtlasBot.DataTypes
{
    public abstract class Commands
    {
        public CommandService commands;

        public Commands(CommandService commands)
        {
            this.commands = commands;
        }

        public abstract void CreateCommands();
    }
}
