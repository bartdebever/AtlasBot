using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;

namespace DataLibary.Repos
{
    public class MessageRepo
    {
        private IMessageContext context;

        public MessageRepo(IMessageContext context)
        {
            this.context = context;
        }
        public List<string> GetAllMessages(ulong serverid)
        {
            return context.GetAllMessages(serverid);
        }

        public string GetDefaultMessage(string code)
        {
            return context.GetDefaultMessage(code);
        }

        public string GetMessage(string code, ulong serverid)
        {
            return context.GetMessage(code, serverid);
        }

        public void AddMessage(string code, ulong serverid, string message)
        {
            context.AddMessage(code, serverid, message);
        }

        public void RemoveMessage(string code, ulong serverid)
        {
            context.RemoveMessage(code, serverid);
        }
    }
}
