using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Data
{
    public interface IMessageContext
    {
        List<string> GetAllMessages(ulong serverid);
        string GetDefaultMessage(string code);
        string GetMessage(string code, ulong serverid);
        void AddMessage(string code, ulong serverid, string message);
        void RemoveMessage(string code, ulong serverid);
    }
}
