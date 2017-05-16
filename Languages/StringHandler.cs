using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.MSSQLContext;
using DataLibary.Repos;
using Discord;

namespace Languages
{
    public class StringHandler
    {
        public string Message { get; private set; }
        private Discord.Server Server { get; set; }
        public string Code { get; private set; }
        private Discord.User User { get; set; }
        private Discord.Role Role { get; set; }

        public StringHandler(Discord.Server server)
        {
            this.Server = server;
        }

        public string Build(string code)
        {
            Code = code;
            GetMessageFromDatabase(Code);
            ReplaceParameters();
            return Message;
        }

        private void GetMessageFromDatabase(string code)
        {
             try
             {
                 //Message = new MessageRepo(new MessageContext()).GetMessage(code, Server.Id);
                 throw new Exception();
             }
              catch
              {
                  Message = new MessageRepo(new MessageContext()).GetDefaultMessage(code);
              }
             
        }

        public void AddObject(Discord.User user)
        {
            User = user;
        }

        public void AddObject(Discord.Role role)
        {
            Role = role;
        }

        private void ReplaceParameters()
        {
            try
            {
                Message = Message.Replace("%user%", User.Name);
            }
            catch
            {
                
            }
            try
            {
                Message = Message.Replace("%server%", Server.Name);
            }
            catch
            {

            }
            try
            {
                Message = Message.Replace("%role", Role.Name);
            }
            catch
            {

            }
        }
    }
}
