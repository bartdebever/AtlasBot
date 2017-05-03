using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Data
{
    public interface IServerContext
    {
        void AddServer(ulong serverid, ulong ownerid, string servername, string key);
        void VerifyServerSQL(ulong userid, string key);
        void AddInviteLink(ulong userid, ulong serverid, string key);
        bool IsAdmin(ulong userid, ulong server);
        List<ulong> GetAllServerIds();
        bool IsServerVerified(ulong serverid);
    }
}
