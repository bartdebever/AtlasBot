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
        string GetServerDescription(ulong serverid);
        void SetServerDecription(ulong serverid, string description);
        bool IsServerVerified(ulong serverid);
        string InviteLink(ulong serverid);
        string ServerName(ulong serverid);
        void SetServerName(ulong serverid, string name);
        void AddAdmin(ulong userid, ulong serverid);
        List<string> ListAdmins(ulong serverid);
        void RemoveAdmin(ulong userid, ulong serverid);
        DateTime GetLastRefreshDate(ulong serverid);
        void SetLastRefreshDate(ulong serverid, DateTime date);
    }
}
