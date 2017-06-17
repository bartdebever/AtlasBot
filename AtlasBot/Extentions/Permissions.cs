using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using Discord.Commands;
using Languages;

namespace AtlasBot.Extentions
{
    public static class Permissions
    {
        public static CommandBuilder AdminCheck(this CommandBuilder builder)
        {
            ServerRepo serverRepo = new ServerRepo(new ServerContext());
            return builder.AddCheck((command, user, channel) => serverRepo.IsAdmin(user.Id, channel.Server.Id) &&
                                                                serverRepo.IsServerVerified(channel.Server.Id), Eng_Default.NotAllowed());
        }
    }
}
