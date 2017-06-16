using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasBot.DataTypes;
using Discord;
using Discord.Commands;

namespace AtlasBot.Modules.Help
{
    public class HelpCommand:Commands
    {
        public HelpCommand(CommandService commands) : base(commands)
        {
            CreateCommands();
        }

        public override void CreateCommands()
        {
            Help();
        }

        public void Help()
        {
            commands.CreateCommand("help")
                .Do(async (e) =>
                {
                    await e.User.SendMessage(
                        "Nearly every command has a help function, you can use this by doing -command ?.\r\n***Modules:***\r\n**Account:**\r\n*-ClaimAccount <Region> <SummonerName>*, use this to claim your League of Legends account within our system. This will make sure you get your roles on every server that uses AtlasBot\r\n*-Claim*, this tries to finish any unclaimed account that you may have set up.\r\n*-GetRoles*, with this command you will gain every role from the server that you are on that is tied to your League of Legends account.\r\n*-Update*, this will update your roles on all servers you are on, you can only use this command daily.\n*-Queue <queue>*, use this command to have a message pasted in the \"looking for group\" chats around all AtlasBot servers.\r\n\r\n**Roles:**\r\n*-Roles <role>*, you can input any role here and the bot will try to give it to you. Examples: -Roles EUW, -Roles Mid, -Roles Diamond\r\n*-Region*, this command can give you a role based around your region, please use *-region ?* for more information.\r\n*-Rank*, this command can give you a role based around your League of Legends rank, please use *-rank ?* for more information.\r\n*-Role*, this command can give you a role based around a League of Legends role, please use *-role ?* for more information\r\n*-Mastery*, this command can give you a role based on your mastery points on the servers champion. **This command can only be used with linked accounts.**");
                    await e.User.SendMessage(
                        "***If you don\'t own a server with AtlasBot on it, feel free to ignore this part.***\r\n**Role management:**\r\n*-Command*, use this command to allow and deny access to things like Rank and Region roles. Please use *-command ?* for more information.\r\n*-Override*, you can use overrides to add custom ranks like \"NSFW\", \"Challenge/Master\" or any other role. This command can also be used to disable roles to be obtainable through commands like \"-Rank master\".\r\nPlease use *-Override ?* for more information or ask Bort#9703.\r\n*-CommandType*, use this feature to set what kind of Rank or Lane system your server is using. Use *-CommandType ?* for more information.\r\n*-LFGChannel*, use this command to set up the Looking For Group feature. Use *-LFGChannel #channel* to set or channel or *-LFGChannel disable* to disable it again.\r\n*-ConfigMastery*, use this command to set up the mastery point system for your server. Use *-ConfigMastery ?* for more information.");
                    if ((Convert.ToInt64(e.Message.Id.ToString()) %10) > 5)
                    {
                        await e.Channel.SendMessage("Slided it into DMs :eyes:");
                    }
                    else
                    {
                        await e.Channel.SendMessage("Thou shall be helped");
                    }
                    
                });
        }
    }
}
