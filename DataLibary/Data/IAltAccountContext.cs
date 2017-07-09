using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Data
{
    public interface IAltAccountContext
    {
        string GetToken(ulong userid);
        void AddAccount(ulong discordid, long riotid, int regionid, string token);
        void VerifyAccount(ulong userid);
        bool UniverifiedAccount(ulong userid);
    }
}
